using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Models;
namespace SmallBackendApi
{
   
    public class AppDbContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SmallBackendDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}