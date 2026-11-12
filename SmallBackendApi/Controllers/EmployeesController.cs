using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Dtos;
using SmallBackendApi.Models;

namespace SmallBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Get all employees, optionally filtered by department.</summary>
        // GET: api/employees?departmentId=1  (query parameter)
        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetAll([FromQuery] int? departmentId)
        {
            var query = _context.Employees.AsNoTracking().AsQueryable();

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            var employees = await query
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .ToListAsync();

            return Ok(employees);
        }

        /// <summary>Get one employee by id.</summary>
        // GET: api/employees/5  (route parameter)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        /// <summary>Create a new employee. Returns 201 with a Location header.</summary>
        // POST: api/employees
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto)
        {
            // The department must exist, otherwise the foreign key fails (400, not 500)
            var department = await _context.Departments.FindAsync(dto.DepartmentId);
            if (department == null)
            {
                return BadRequest("Department does not exist.");
            }

            // 409 Conflict: an employee with the same name already exists
            bool nameExists = await _context.Employees.AnyAsync(e => e.Name == dto.Name);
            if (nameExists)
            {
                return Conflict("An employee with this name already exists.");
            }

            var employee = new Employee
            {
                Name = dto.Name,
                Salary = dto.Salary,
                DepartmentId = dto.DepartmentId,
                HireDate = DateTime.Now
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                DepartmentName = department.Name
            };

            // 201 Created + Location header pointing to GET api/employees/{id}
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, result);
        }

        /// <summary>Update an existing employee.</summary>
        // PUT: api/employees/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            bool departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest("Department does not exist.");
            }

            employee.Name = dto.Name;
            employee.Salary = dto.Salary;
            employee.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>Delete an employee.</summary>
        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}