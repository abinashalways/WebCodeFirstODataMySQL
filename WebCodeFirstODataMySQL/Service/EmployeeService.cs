using Microsoft.AspNetCore.Mvc;
using WebCodeFirstODataMySQL.Models;
using WebCodeFirstODataMySQL.Repository;

namespace WebCodeFirstODataMySQL.Service
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<EmployeeDto>> GetEmployees()
        {
           return await _employeeRepository.GetEmployees();
        }

      public async Task<IActionResult> GetPhoto(Guid empId)
        {
            return await _employeeRepository.GetPhoto(empId);
        }

       public async Task<EmployeeDto> GetEmployee(Guid id)
        {
            return await _employeeRepository.GetEmployee(id);
        }
        public async Task<IActionResult> GetCount()
        { 
            return await _employeeRepository.GetCount();
        }
        public async Task<IActionResult> CreateAll([FromForm] Employee? employee, IFormFile? file)
        {
            return await _employeeRepository.CreateAll(employee, file);
        }
        public async Task<IActionResult> CreateEmployee([FromBody] Employee? employee)
        {
            return await _employeeRepository.CreateEmployee(employee);
        }

        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            return await _employeeRepository.CreateDepartment(department);
        }
        public async Task<IActionResult> CreateLocation([FromBody] Location location)
        {
            return await _employeeRepository.CreateLocation(location);
        }

        public async Task<IActionResult> Update([FromForm] Employee? employee, IFormFile? file, Guid id)
        {
            return await _employeeRepository.Update(employee, file, id);
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            return await _employeeRepository.Delete(id);
        }

    }
}
