using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using WebCodeFirstODataMySQL.Database_Context;

using WebCodeFirstODataMySQL.Models;
using static System.Net.WebRequestMethods;

namespace WebCodeFirstODataMySQL.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmpDetailsContext _context;

        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private  readonly IConfiguration Configuration;
       
       


        public EmployeeRepository
   (EmpDetailsContext context, IUrlHelperFactory urlHelperFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;

            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
            Configuration = configuration;


        }


        public async Task<List<EmployeeDto>> GetEmployeesFromOData()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext
            {
                HttpContext = _httpContextAccessor.HttpContext!
            });


            var odataQueryString = $"$filter=Department/Location ne null" +
                          
                          
                           $"&$count=true" +
                           $"&$orderby=DOJ desc" +
                           $"&$top=10" +
                           $"&$skip=0";


            var baseUri = _httpContextAccessor.HttpContext!.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + "/odata/Employee/GetEmployees";
            var odataQueryUri = new Uri($"{baseUri}?{odataQueryString}");

            
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(odataQueryUri);

            if (response.IsSuccessStatusCode)
            {
                var employeesJson = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesJson);

              
                var employeeDtos = employees!.Select(e => new EmployeeDto
                {
                    EmpId = e.EmpId,
                    EName = e.EName,
                    Designation = e.Designation,
                    DOJ = e.DOJ,
                    Salary = e.Salary,
                    PhotoUrl = urlHelper.Action("GetPhoto", "Employee", new { empId = e.EmpId }, _httpContextAccessor.HttpContext!.Request.Scheme),
                    DeptID = e.DeptID,
                    Department = e.Department != null ? new DepartmentDto
                    {
                        DName = e.Department.DName,
                        LocationID = e.Department.LocationID,
                        Location = e.Department.Location != null ? new LocationDto
                        {
                            LocationName = e.Department.Location.LocationName,
                            Country = e.Department.Location.Country
                        } : null
                    } : null
                }).ToList();

                return employeeDtos;
            }

            return new List<EmployeeDto>();
        }















        public async Task<List<EmployeeDto>> GetEmployees()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext
            {
                HttpContext = _httpContextAccessor.HttpContext!
            });
            
           
            var employeeDtos = await _context.Employee
                .Include(e => e.Department)              
                .ThenInclude(d => d!.Location)             
                .Select(e => new EmployeeDto
                {
                    EmpId = e.EmpId,
                    EName = e.EName,
                    Designation = e.Designation,
                    DOJ = e.DOJ,
                    Salary = e.Salary,
                    PhotoUrl = urlHelper.Action("GetPhoto", "Employee", new { empId = e.EmpId }, _httpContextAccessor.HttpContext!.Request.Scheme),
                    DeptID = e.DeptID,
                    Department = e.Department != null ? new DepartmentDto
                    {
                        DName = e.Department.DName,
                        LocationID = e.Department.LocationID,
                        Location = e.Department.Location != null ? new LocationDto
                        {
                            LocationName = e.Department.Location.LocationName,
                            Country = e.Department.Location.Country
                        } : null
                    } : null
                }).ToListAsync();

            return employeeDtos;
        }








        public async Task<EmployeeDto> GetEmployee(Guid id)
        {
            var employee = await _context.Employee
                .Include(e => e.Department)
                .ThenInclude(d => d!.Location)
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (employee == null)
            {
                throw new NotImplementedException("Employee not found");
            }

            // Use UrlHelper to generate the photo URL
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext
            {
                HttpContext = _httpContextAccessor.HttpContext!
            });




            // return employee;  // for directly getting values without mapping


            var employeeDto = new EmployeeDto
            {
                EmpId = employee.EmpId,
                EName = employee.EName,
                Designation = employee.Designation,
                Email = employee.Email,
                ContactNo = employee.ContactNo,
                DOJ = employee.DOJ,
                Salary = employee.Salary,
                PhotoUrl = urlHelper.Action("GetPhoto", "Employee", new { empId = employee.EmpId }, _httpContextAccessor.HttpContext!.Request.Scheme),
                DeptID = employee.DeptID,
                Department = new DepartmentDto
                {
                    DName = employee.Department?.DName,
                    LocationID = employee.Department?.LocationID,
                    Location = new LocationDto
                    {
                        LocationName = employee.Department?.Location?.LocationName,
                        Country = employee.Department?.Location?.Country
                    }
                }
            };

            return employeeDto;
        }
    
        public async Task<int> GetCount()
        {
            var count = await _context.Employee.Include(e => e.Department).ThenInclude(d => d!.Location).CountAsync();
                    //return new OkObjectResult( count);
                    return count;
        }

        public async Task<IActionResult> CreateAll([FromForm] Employee? employee, IFormFile? file)
        {
            if (employee == null)
            {
                throw new NotImplementedException ("Employee data is required.");
            }

            byte[]? filedata = null;
            //if (file.Length > 5 * 1024 * 1024) // it is for 5 MB
            //{
            //    return BadRequest("File size exceeds the limit.");
            //}

            
            var allowedExtensions = Configuration!.GetSection("AllowedExtensions").Get<string[]>();
            var extension = file != null ? Path.GetExtension(file.FileName).ToLowerInvariant() : string.Empty;



            if (file != null && file.Length > 0)
            {
                if (!allowedExtensions!.Contains(extension))
                {
                    throw new NotImplementedException("Invalid file type.");
                }

                using (var memorystream = new MemoryStream())
                {
                    await file.CopyToAsync(memorystream);
                    filedata = memorystream.ToArray();
                }
                employee.Photo = filedata;
            }
            else
            {
                employee.Photo = filedata;
                //  return BadRequest("File is required");

            }
            employee.EmpId = Guid.NewGuid();
            employee.DeptID= Guid.NewGuid();
       
            employee.Department!.DeptID = employee.DeptID;
            employee.Department.LocationID= Guid.NewGuid();

            employee.Department.Location!.LocationID = employee.Department.LocationID;

            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();
            return new OkObjectResult(employee);
        }

      public async  Task<IActionResult> CreateLocation([FromBody] Location location)
        {
            if (location == null)
            {
                throw new NotImplementedException("Location data is required.");
            }
            location.LocationID = Guid.NewGuid();
            _context.Location.Add(location);
            await _context.SaveChangesAsync();
            return new OkObjectResult(location);
        }

     public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            if (department == null)
            {
                throw new NotImplementedException("Department data is required.");
            }

            var existingDepartment = await _context.Department.FindAsync(department.LocationID);

            if (existingDepartment != null)
            {
                department.LocationID = existingDepartment.LocationID;
            }
            else
            {
                department.DeptID= Guid.NewGuid();
                department.LocationID= Guid.NewGuid();
                _context.Department.Add(department);
            }
            await _context.SaveChangesAsync();
            return new OkObjectResult(department);
     }
      public async  Task<IActionResult> CreateEmployee([FromBody] Employee? employee)
        {
            if (employee == null)
            {
                throw new NotImplementedException("Employee data is required.");
            }

            byte[]? filedata = null;
            //if (file.Length > 5 * 1024 * 1024) // it is for 5 MB
            //{
            //    return BadRequest("File size exceeds the limit.");
            //}
            
           
            var allowedExtensions = Configuration!.GetSection("AllowedExtensions").Get<string[]>();

          //  var extension = file != null ? Path.GetExtension(file.FileName).ToLowerInvariant() : string.Empty;



            //if (file != null && file.Length > 0)
            //{
            //    if (!allowedExtensions!.Contains(extension))
            //    {
            //        throw new NotImplementedException("Invalid file type.");
            //    }

            //    using (var memorystream = new MemoryStream())
            //    {
            //        await file.CopyToAsync(memorystream);
            //        filedata = memorystream.ToArray();
            //    }
            //    employee.Photo = filedata;
            //}
         //   else
            {
                employee.Photo = filedata;
                //  return BadRequest("File is required");

            }
            var existingEmployee = await _context.Employee.FindAsync(employee.DeptID);
           
            if (existingEmployee != null)
            {
                employee.DeptID = existingEmployee.DeptID;
            }
            else
            {
                employee.EmpId = Guid.NewGuid();
                employee.DeptID = Guid.NewGuid();
                _context.Employee.Add(employee);
            }
            
            await _context.SaveChangesAsync();
            return new OkObjectResult(employee);
        }
        public async Task<FileResult> GetPhoto(Guid empId)
        {
            var employee = await _context.Employee.FindAsync(empId);

            if (employee == null || employee.Photo == null)
            {
                throw new NotImplementedException ("Photo not found.");
            }

            //return new FileContentResult(employee.Photo, "image/jpeg");    

           

            return GetPic(employee.Photo);
        }

        public FileResult GetPic(byte[] b)
        {

            return new FileContentResult(b, "image/jpeg");
        }





        public async Task<IActionResult> Delete(Guid id)
        {
            var employee =await _context.Employee.Include(e=>e.Department).ThenInclude(d=>d!.Location).FirstOrDefaultAsync(e=>e.EmpId==id);

            if (employee == null)
            {
                // _logger.LogWarning("Attempt to delete non-existing employee with ID {Id}.", id);
                throw new NotImplementedException($"Employee with ID {id} not found.");
            }
            if (employee.Department != null)
            {
                if (employee.Department.Location != null)
                {
                    _context.Location.Remove(employee.Department.Location); 
                }
                _context.Department.Remove(employee.Department); 
            }
            _context.Employee.Remove(employee);
          await  _context.SaveChangesAsync();
           
            
            return new OkObjectResult(await GetEmployees());

        }


        public async Task<IActionResult> Update([FromForm] Employee? employee, IFormFile? file, Guid id)
        {
            if (id != employee!.EmpId)
            {
                throw new NotImplementedException("Employee ID mismatch.");
            }

            var existingEmployee = await _context.Employee
                .Include(d => d.Department)
                .ThenInclude(l => l!.Location)
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (existingEmployee == null)
            {
                throw new NotImplementedException($"Employee with ID {id} not found.");
            }

            byte[]? filedata = null;
            var allowedExtensions = Configuration!.GetSection("AllowedExtensions").Get<string[]>();
            var extension = file != null ? Path.GetExtension(file.FileName).ToLowerInvariant() : string.Empty;

            if (file != null && file.Length > 0)
            {
                if (!allowedExtensions!.Contains(extension))
                {
                    throw new NotImplementedException("Invalid file type.");
                }
                using (var memorystream = new MemoryStream())
                {
                    await file.CopyToAsync(memorystream);
                    filedata = memorystream.ToArray();
                }
                existingEmployee.Photo = filedata;
            }

            // Only update fields if they are provided
            if (!string.IsNullOrEmpty(employee.EName))
            {
                existingEmployee.EName = employee.EName;
            }

            if (!string.IsNullOrEmpty(employee.Designation))
            {
                existingEmployee.Designation = employee.Designation;
            }

            if (!string.IsNullOrEmpty(employee.Email))
            {
                existingEmployee.Email = employee.Email;
            }

            if (employee.ContactNo.HasValue)
            {
                existingEmployee.ContactNo = employee.ContactNo;
            }

            if (employee.DOJ.HasValue)
            {
                existingEmployee.DOJ = employee.DOJ;
            }

            if (employee.Salary.HasValue)
            {
                existingEmployee.Salary = employee.Salary;
            }

            if (employee.DeptID.HasValue)
            {
                existingEmployee.DeptID = employee.DeptID;
            }

            // Update Department and Location if provided
            if (existingEmployee.Department != null)
            {
                if (!string.IsNullOrEmpty(employee.Department?.DName))
                {
                    existingEmployee.Department.DName = employee.Department.DName;
                }

                if (employee.Department?.LocationID.HasValue == true)
                {
                    existingEmployee.Department.LocationID = employee.Department.LocationID.Value;
                }

                if (!string.IsNullOrEmpty(employee.Department?.Location?.LocationName))
                {
                    existingEmployee.Department.Location!.LocationName = employee.Department.Location.LocationName;
                }

                if (!string.IsNullOrEmpty(employee.Department?.Location?.Country))
                {
                    existingEmployee.Department.Location!.Country = employee.Department.Location.Country;
                }
            }

            _context.Entry(existingEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                Message = "Employee updated successfully.",
                UpdatedEmployee = employee,
              
            });
        }


    }
}
