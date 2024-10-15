using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebCodeFirstODataMySQL.Models
{
   // [Table("Employee",Schema ="dbo")]
    public class Employee
    {
        [Key]
       // [Required(ErrorMessage = "Employee ID is Required")]
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       public Guid? EmpId { get; set; }

       // [Required(ErrorMessage = "Name is Required")]
        [StringLength(50, ErrorMessage = "Name should be within 50 characters")]
        public string? EName { get; set; }
       
        public string? Designation { get; set; }

       // [Required(ErrorMessage = "Email is Required")]

        [EmailAddress(ErrorMessage ="Please type correct Mail Id Format")]
        public string? Email { get; set; }

      //  [Required(ErrorMessage = "Contact Number is Required")]

        public long? ContactNo { get; set; }

        public DateTime? DOJ { get; set; }

        public double? Salary { get; set; }
        public byte[]? Photo { get; set; }

       // [Required]
        [ForeignKey("Department")]
        public Guid? DeptID { get; set; }

        // Navigation property
    
        public Department? Department { get; set; }

    }
   // [Table("Department", Schema = "dbo")]
    public class Department
    {
        [Key]
       // [Required(ErrorMessage = "Department Number is Required")]
      //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? DeptID { get; set; }

       // [Required(ErrorMessage = "Department Name is Required")]
        [StringLength(100, ErrorMessage = "Department Name should be within 100 characters")]
        public string? DName { get; set; }
       // [Required]
        [ForeignKey("Location")]
        public Guid? LocationID { get; set; }

        // Navigation property
        public Location? Location { get; set; }
        public List<Employee>? Employees { get; set; }

    }
   // [Table("Location", Schema = "dbo")]
    public class Location
    {
        [Key]
       // [Required(ErrorMessage = "Location Number is Required")]
      //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? LocationID { get; set; }
        public string? LocationName { get; set; }
        public string? Country {  get; set; }
        // Navigation property
         public List<Department>? Departments { get; set; }
    }

}
