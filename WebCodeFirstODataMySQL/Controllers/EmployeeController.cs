using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebCodeFirstODataMySQL.Database_Context;
using WebCodeFirstODataMySQL.Models;
using WebCodeFirstODataMySQL.Repository;
using WebCodeFirstODataMySQL.Service;

namespace WebCodeFirstODataMySQL.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    // [Authorize]
    public class EmployeeController : ODataController
    {
        private readonly IEmployeeService _service;
        private readonly IUrlHelperFactory _urlHelperFactory;
        public EmployeeController(IEmployeeService repository, IUrlHelperFactory urlHelperFactory)
        {
            _service = repository;
            _urlHelperFactory = urlHelperFactory;
        }

        //public async Task<IActionResult> GetEmployees()
        //{
        //    var employees = await _Context.Employee
        //        .Include(e => e.Department)
        //        .ThenInclude(d => d!.Location)
        //        .Select(e => new
        //        {
        //            e.EmpId,
        //            e.EName,
        //            e.Designation,
        //            e.Email,
        //            e.ContactNo,
        //            e.DOJ,
        //            e.Salary,
        //            PhotoUrl = Url.Action("GetPhoto", "Employee", new { empId = e.EmpId }, Request.Scheme)

        //            ,

        //            e.DeptID,
        //            Department = new
        //            {
        //                e.Department!.DName,
        //                e.Department.LocationID,
        //                Location = new
        //                {
        //                    e.Department.Location!.LocationName,
        //                    e.Department.Location.Country
        //                }
        //            }
        //        })
        //        .ToListAsync();

        //    if (employees == null || !employees.Any())
        //    {
        //        return NotFound("No employees found.");
        //    }

        //    return Ok(employees);
        //}

        [EnableQuery]
        [HttpGet]
        [Route("GetEmployees")]
        public async Task<IActionResult> GetEmployees() 
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);
            var employees = await _service.GetEmployees();
            return Ok (employees);
            
            
        }


     

        /*
         // This one is suitable when we do not deal with custom exception handling it gives us particular type of data with its return type 


          [EnableQuery]
          [HttpGet]
          [ResponseCache(Duration = 60)]

          public async Task<ActionResult<IEnumerable<Employee>>> Get()
          {
              var employees = await _Context.Employee.Include(e=>e.Department).ThenInclude(d=>d!.Location).ToListAsync();
              if (employees == null || !employees.Any()) { return NotFound("No employees found."); }
              return Ok(employees);
          }
        */

        //[HttpGet("GetPhoto/{empId}")]
        //public async Task<IActionResult> GetPhoto(int empId)
        //{
        //    var employee = await _Context.Employee.FindAsync(empId);

        //    if (employee == null || employee.Photo == null)
        //    {
        //        return NotFound("Photo not found.");
        //    }

        //    return File(employee.Photo, "image/jpeg");
        //}


        [HttpGet("GetPhoto")]
        public async Task<FileResult> GetPhoto(Guid empId)
        {
            return await _service.GetPhoto(empId);

            
        }














        // [EnableQuery]
        ////  [HttpGet("GetEmployee/{id}")]
        //  [ResponseCache(Duration = 60)]

        //public IActionResult GetEmployee([FromODataUri]int id)
        //{
        //    var employee = _Context.Employee.Include(e=>e.Department).ThenInclude(d=>d!.Location).FirstOrDefault(e=>e.EmpId==id);


        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(employee);
        //}

        [EnableQuery]
          [HttpGet("GetEmployee")]
       // [ResponseCache(Duration = 60)]

        public async Task< IActionResult> GetEmployee([FromODataUri] Guid id)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);
            var employee = await _service.GetEmployee(id);  
            return Ok(employee);
        }




        //[EnableQuery]
        //[HttpGet("$count")]
        //public async Task<IActionResult> GetCount()
        //{
        //    try
        //    {
        //        var count = await _Context.Employee.Include(e => e.Department).ThenInclude(d=>d!.Location).CountAsync();
        //        return Ok(count);
        //    }
        //    catch (Exception )
        //    {
        //     //   _logger.LogError(ex, "An error occurred while fetching the employee count.");
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        //    }

        //}

      //  [EnableQuery]
        [HttpGet("$count")]
        public async Task<int> GetCount()
        {               
                return await _service.GetCount();
       
        }


        //[HttpPost]
        //[Route("Create")]
        //public async Task<IActionResult> Create([FromForm] Employee employee,IFormFile file)
        //{
        //    if (employee == null )
        //    {
        //        return BadRequest("Employee data is required.");
        //    }

        //    byte[] ?filedata=null ;
        //    //if (file.Length > 5 * 1024 * 1024) // Limit to 5 MB
        //    //{
        //    //    return BadRequest("File size exceeds the limit.");
        //    //}

        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        //    var extension =file!=null? Path.GetExtension(file.FileName).ToLowerInvariant():string.Empty;



        //    if (file != null && file.Length > 0)
        //    {
        //        if (!allowedExtensions.Contains(extension))
        //        {
        //            return BadRequest("Invalid file type.");
        //        }

        //        using (var memorystream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(memorystream);
        //            filedata = memorystream.ToArray();
        //        }
        //        employee.Photo = filedata;
        //    }
        //    else
        //    {
        //        employee.Photo = filedata;
        //      //  return BadRequest("File is required");

        //    }

        //    _Context.Employee.Add(employee);
        //    await _Context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmpId }, employee);
        //}


        [HttpPost]
        [Route("CreateAll")]
        public async Task<IActionResult> CreateAll([FromForm] Employee? employee, IFormFile? file)
        {
            
            return Ok (await _service.CreateAll(employee,file));
            
           // return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmpId }, employee);
        }
        [HttpPost]
        [Route("CreateLocation")]
        public async Task<IActionResult> CreateLocation([FromBody] Location location)
        {
            return Ok(await _service.CreateLocation(location));
        }
        [HttpPost]
        [Route("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            return Ok(await _service.CreateDepartment(department));
        }
        [HttpPost]
        [Route("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee? employee)
        {
            return Ok(await _service.CreateEmployee(employee!));
        }

        // [HttpPut("Update/{id}")]
        // public async Task<IActionResult> Update([FromForm] Employee employee,IFormFile file,int id)
        // {
        //     if (id != employee.EmpId)
        //     {
        //         return BadRequest("Employee ID mismatch.");
        //     }
        //     var existingEmployee = await _Context.Employee.Include(d=>d.Department).ThenInclude(l=>l!.Location).FirstOrDefaultAsync(e=>e.EmpId==id);
        //     if (existingEmployee == null)
        //     {
        //      //   _logger.LogWarning("Attempt to update non-existing employee with ID {Id}.", id);
        //         return NotFound($"Employee with ID {id} not found.");
        //     }
        //     byte[] ?filedata = null;
        //     var allowedExtensions = new[] { ".png", ".jpeg", ".jpg" };
        //     var extension = file != null ? Path.GetExtension(file.FileName).ToLowerInvariant() : string.Empty;

        //     if(file != null && file.Length > 0)
        //     {
        //         if(!allowedExtensions.Contains(extension))
        //         {
        //             return BadRequest("Invalid file type.");
        //         }
        //         using (var memorystream = new MemoryStream())
        //         {
        //             await file.CopyToAsync(memorystream);
        //             filedata = memorystream.ToArray();
        //         }
        //         existingEmployee.Photo = filedata;

        //     }
        //     else
        //     {
        //         existingEmployee.Photo = existingEmployee.Photo;
        //     }
        //     existingEmployee.EName = employee.EName;
        //     existingEmployee.Designation = employee.Designation;
        //     existingEmployee.Email = employee.Email;
        //     existingEmployee.ContactNo = employee.ContactNo;
        //     existingEmployee.DOJ = employee.DOJ;
        //     existingEmployee.Salary = employee.Salary;
        //     existingEmployee.DeptID = employee.DeptID;

        ///*     if(existingEmployee.Department!.DeptID == employee.DeptID) {
        //         existingEmployee.Department!.DeptID = employee.Department!.DeptID;
        //         existingEmployee.Department!.DName=employee.Department!.DName;
        //         existingEmployee.Department!.LocationID = employee.Department!.LocationID;
        //     }

        //     if (existingEmployee.Department.Location!.LocationID == employee.Department!.LocationID)
        //     {

        //         existingEmployee.Department!.Location.LocationID = employee.Department.Location!.LocationID;
        //         existingEmployee.Department!.Location.LocationName=employee.Department!.Location.LocationName;
        //         existingEmployee.Department!.Location.Country=employee.Department!.Location.Country;
        //             }
        //*/
        //     // Update Department if changed
        //     if (existingEmployee.DeptID != employee.DeptID)
        //     {
        //         var department = await _Context.Department.FindAsync(employee.DeptID);
        //         if (department == null)
        //         {
        //             return BadRequest("Invalid Department ID.");
        //         }
        //         existingEmployee.Department = department;
        //     }

        //     // Update Location if Department is changed or LocationID is different
        //     if (existingEmployee.Department != null && employee.Department?.LocationID != existingEmployee.Department.LocationID)
        //     {
        //         var location = await _Context.Location.FindAsync(employee.Department!.LocationID);
        //         if (location == null)
        //         {
        //             return BadRequest("Invalid Location ID.");
        //         }
        //         existingEmployee.Department.Location = location;
        //     }


        //     _Context.Entry(existingEmployee).State = EntityState.Modified;
        //     await _Context.SaveChangesAsync();
        //     return Ok(new
        //     {
        //         Message = "Employee updated successfully.",
        //         UpdatedEmployee = employee,

        //     });

        // } 



        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromForm] Employee? employee, IFormFile? file, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var employe = await _service.Update(employee,file!,id);
            return Ok(employe);
           

        }

       


        //[HttpDelete("Delete/{id}")]
        //public  async Task< IActionResult> Delete(int id)
        //{
        //    var employee = _Context.Employee.Find(id);

        //    if (employee == null)
        //    {
        //       // _logger.LogWarning("Attempt to delete non-existing employee with ID {Id}.", id);
        //        return NotFound($"Employee with ID {id} not found.");
        //    }

        //    _Context.Employee.Remove(employee);
        //    _Context.SaveChanges();
        //    return await GetEmployees();
        //}



        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);
            var employees = await _service.Delete(id);
            return Ok(employees);
        }













    }

}
