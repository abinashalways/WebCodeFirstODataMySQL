using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using WebCodeFirstODataMySQL.Models;

namespace WebCodeFirstODataMySQL.Repository
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetEmployees();
        Task<FileResult> GetPhoto(Guid empId);

        Task<Employee> GetEmployee(Guid id);
        Task<int> GetCount();
        Task<IActionResult> CreateAll([FromForm] Employee? employee, IFormFile? file);
        Task<IActionResult> CreateEmployee([FromBody] Employee? employee);

        Task<IActionResult> CreateDepartment([FromBody] Department department);
        Task<IActionResult> CreateLocation([FromBody] Location location);

        Task<IActionResult> Update([FromForm] Employee? employee, IFormFile? file, Guid id);
        Task<IActionResult> Delete(Guid id);
    }
}
