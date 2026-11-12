using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SmallBackendApi.Models;

namespace SmallBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        // The database context is injected here automatically
        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public IActionResult GetAll()
        {
            // Includes the Department data when fetching employees
            var employees = _context.Employees.Include(e => e.Department).ToList();
            return Ok(employees);
        }

        // POST: api/employees
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return Ok(employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Employee updatedEmployee)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null) return NotFound();

            employee.Name = updatedEmployee.Name;
            employee.DepartmentId = updatedEmployee.DepartmentId;

            _context.SaveChanges();
            return Ok(employee);
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return Ok("Employee deleted successfully");
        }
    }
}