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

        // Fluent API: configure the relationships and the decimal precision
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Department 1 -> many Employees
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            // Employee 1 -> many Tasks
            modelBuilder.Entity<EmployeeTask>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.Tasks)
                .HasForeignKey(t => t.EmployeeId);

            // Fix the decimal warning for Salary
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);
        }
    }
}