using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Models;

namespace SmallBackendApi.Controllers
{
    [Route("api/ef/departments")]
    [ApiController]
    public class EfDepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        // The DbContext is given to us by dependency injection
        public EfDepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // ADD: create a new department
        [HttpPost]
        public async Task<IActionResult> AddDepartment(string name)
        {
            var department = new Department { Name = name };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return Ok(department);
        }

        // FIND: get one department by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        // READ ALL: get all departments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _context.Departments.ToListAsync();
            return Ok(departments);
        }

        // UPDATE: change the name of a department
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, string newName)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            department.Name = newName;
            await _context.SaveChangesAsync();

            return Ok(department);
        }

        // REMOVE: delete a department
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}