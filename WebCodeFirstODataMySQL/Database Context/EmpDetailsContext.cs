using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using WebCodeFirstODataMySQL.Models;

namespace WebCodeFirstODataMySQL.Database_Context
{
    public class EmpDetailsContext : DbContext    
    {
        public EmpDetailsContext(DbContextOptions<EmpDetailsContext> options): base(options) { }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Location> Location { get; set; }

        // For stored procedure case

        //public async Task<int> AddEmployeeAsync(int empId, string eName, string designation, string email, long contactNo, decimal salary, int deptID)
        //{
        //    var parameters = new[]
        //    {
        //    new MySqlParameter("@empId", empId),
        //    new MySqlParameter("@eName", eName),
        //    new MySqlParameter("@designation", designation),
        //    new MySqlParameter("@email", email),
        //    new MySqlParameter("@contactNo", contactNo),
        //    new MySqlParameter("@salary", salary),
        //    new MySqlParameter("@deptID", deptID)
        //};

        //    return await Database.ExecuteSqlRawAsync("CALL AddEmployee(@empId, @eName, @designation, @email, @contactNo, @salary, @deptID)", parameters);
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e =>e.DeptID);

            //another
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmpId);

            modelBuilder.Entity<Employee>()
                .Property(e => e.EName).IsRequired();

            modelBuilder.Entity<Employee>()
                .Property(e => e.Email).IsRequired();

            modelBuilder.Entity<Employee>()
                .Property(e => e.ContactNo).IsRequired();

            // modelBuilder.Entity<Department>().HasMany(d => d.Employees).WithOne(e => e.Department).HasForeignKey(e => e.DeptID);
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Location)
                .WithMany(l => l.Departments)
                .HasForeignKey(d => d.LocationID);
            // modelBuilder.Entity<Location>().HasMany(l=>l.Departments).WithOne(d=>d.Location).HasForeignKey(d => d.LocationID);
            // modelBuilder.Entity<Department>().HasIndex(d => d.DName).IsUnique();

            modelBuilder.Entity<Employee>()
        .HasOne(e => e.Department)
        .WithMany(d => d.Employees)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Location)
                .WithMany(l => l.Departments)
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }
    }
}

