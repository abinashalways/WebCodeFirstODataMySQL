using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace WebCodeFirstODataMySQL.Models
   
{
    public class EmployeeDto
    {
        
        public Guid? EmpId { get; set; }
        public string? EName { get; set; }
        public string? Designation { get; set; }
        public string? Email { get; set; }
        public long? ContactNo { get; set; }
        public DateTime? DOJ { get; set; }
        public double? Salary { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid? DeptID { get; set; }
        public DepartmentDto? Department { get; set; }
    }

    public class DepartmentDto
    {
        public string? DName { get; set; }
        public Guid? LocationID { get; set; }
        public LocationDto? Location { get; set; }
    }

    public class LocationDto
    {
        public  string? LocationName { get; set; }
        public  string? Country { get; set; }
        
        
    }

}
