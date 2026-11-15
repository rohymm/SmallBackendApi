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
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(AppDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>Get all employees, optionally filtered by department.</summary>
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
                    Email = e.Email,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .ToListAsync();

            return Ok(employees);
        }

        /// <summary>Get one employee by id.</summary>
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
                    Email = e.Email,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                // Expected error: the client asked for something that does not exist
                _logger.LogWarning("Employee {EmployeeId} was not found", id);
                return NotFound();
            }

            return Ok(employee);
        }

        /// <summary>Create a new employee. Returns 201 with a Location header.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto)
        {
            // Business validation: the department must exist
            var department = await _context.Departments.FindAsync(dto.DepartmentId);
            if (department == null)
            {
                _logger.LogWarning("Create employee failed: department {DepartmentId} does not exist", dto.DepartmentId);
                return BadRequest("Department does not exist.");
            }

            // Business validation: the email must be unique
            bool emailExists = await _context.Employees.AnyAsync(e => e.Email == dto.Email);
            if (emailExists)
            {
                _logger.LogWarning("Create employee failed: email already used");
                return Conflict("An employee with this email already exists.");
            }

            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Salary = dto.Salary,
                DepartmentId = dto.DepartmentId,
                HireDate = DateTime.Now
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Log the id only, not the email (avoid sensitive data in logs)
            _logger.LogInformation("Employee {EmployeeId} was created in department {DepartmentId}", employee.Id, employee.DepartmentId);

            var result = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                DepartmentName = department.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, result);
        }

        /// <summary>Update an existing employee.</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Update failed: employee {EmployeeId} was not found", id);
                return NotFound();
            }

            bool departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest("Department does not exist.");
            }

            // The email must stay unique (ignore the employee we are updating)
            bool emailUsed = await _context.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (emailUsed)
            {
                return Conflict("Another employee already uses this email.");
            }

            employee.Name = dto.Name;
            employee.Email = dto.Email;
            employee.Salary = dto.Salary;
            employee.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Employee {EmployeeId} was updated", id);

            return NoContent();
        }

        /// <summary>Delete an employee.</summary>
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

            _logger.LogInformation("Employee {EmployeeId} was deleted", id);

            return NoContent();
        }
        
    }
}