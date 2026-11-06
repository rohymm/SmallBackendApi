using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Models;

namespace SmallBackendApi
{
    public class AppDbContext : DbContext
    {
        // The options come from Program.cs (dependency injection)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Each DbSet is a table in the database
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTask> EmployeeTasks { get; set; }
    }
}