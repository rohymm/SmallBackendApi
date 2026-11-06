using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Models;

namespace SmallBackendApi.Controllers
{
    [Route("api/ef/employees")]
    [ApiController]
    public class EfEmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EfEmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // WHERE + ORDER BY: employees of one department, highest salary first
        [HttpGet("by-department/{departmentName}")]
        public async Task<IActionResult> GetByDepartment(string departmentName)
        {
            var employees = await _context.Employees
                .Where(e => e.Department.Name == departmentName)
                .OrderByDescending(e => e.Salary)
                .ToListAsync();

            return Ok(employees);
        }

        // PROJECTION (Select): return only the columns we need, not the whole entity
        [HttpGet("names-and-salaries")]
        public async Task<IActionResult> GetNamesAndSalaries()
        {
            var result = await _context.Employees
                .Select(e => new { e.Name, e.Salary })
                .ToListAsync();

            return Ok(result);
        }

        // ANY: does this department have at least one employee?
        [HttpGet("department-has-employees/{departmentName}")]
        public async Task<IActionResult> DepartmentHasEmployees(string departmentName)
        {
            bool exists = await _context.Employees
                .AnyAsync(e => e.Department.Name == departmentName);

            return Ok(exists);
        }

        // COUNT: how many employees earn more than a given salary
        [HttpGet("count-above/{salary}")]
        public async Task<IActionResult> CountAbove(decimal salary)
        {
            int count = await _context.Employees
                .CountAsync(e => e.Salary > salary);

            return Ok(count);
        }

        // FirstOrDefaultAsync: find one employee by name
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Name == name);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // INCLUDE: load Employee + Department + Tasks together
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Tasks)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // THENINCLUDE: Department -> its Employees -> their Tasks
        [HttpGet("department-tree/{departmentName}")]
        public async Task<IActionResult> GetDepartmentTree(string departmentName)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                    .ThenInclude(e => e.Tasks)
                .FirstOrDefaultAsync(d => d.Name == departmentName);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        // AsNoTracking: read-only query, EF does not track the results (faster)
        [HttpGet("read-only")]
        public async Task<IActionResult> GetReadOnly()
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .ToListAsync();

            return Ok(employees);
        }

        // REVIEW CHALLENGE: IT employees with incomplete tasks, salary descending
        [HttpGet("it-with-incomplete-tasks")]
        public async Task<IActionResult> GetItWithIncompleteTasks()
        {
            var employees = await _context.Employees
                .Include(e => e.Tasks)
                .Where(e => e.Department.Name == "IT" && e.Tasks.Any(t => !t.IsCompleted))
                .OrderByDescending(e => e.Salary)
                .ToListAsync();

            return Ok(employees);
        }

        // TRACKING DEMO: update a tracked entity vs a no-tracking entity
        [HttpPut("tracking-demo/{id}")]
        public async Task<IActionResult> TrackingDemo(int id)
        {
            // Tracked query: EF remembers the object, so SaveChanges sees the change
            var tracked = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            // No-tracking query: EF does NOT remember this object
            var noTracking = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

            if (tracked == null || noTracking == null)
            {
                return NotFound();
            }

            tracked.Salary += 100;      // change will be saved
            noTracking.Salary += 5000;  // change will be ignored

            int changes = await _context.SaveChangesAsync();

            return Ok(new
            {
                RowsSaved = changes,
                TrackedSalary = tracked.Salary,
                NoTrackingSalary = noTracking.Salary
            });
        }

        // TRANSACTION: create an employee and his first task, all or nothing
        [HttpPost("with-task")]
        public async Task<IActionResult> CreateEmployeeWithTask(string name, decimal salary, int departmentId, string taskTitle)
        {
            // Start the transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var employee = new Employee
                {
                    Name = name,
                    Salary = salary,
                    DepartmentId = departmentId,
                    HireDate = DateTime.Now
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync(); // now employee.Id is generated

                var task = new EmployeeTask
                {
                    Title = taskTitle,
                    EmployeeId = employee.Id
                };

                _context.EmployeeTasks.Add(task);
                await _context.SaveChangesAsync();

                // Everything worked, so save permanently
                await transaction.CommitAsync();

                return Ok("Employee and task created");
            }
            catch (Exception)
            {
                // Something failed, so undo everything
                await transaction.RollbackAsync();
                return StatusCode(500, "Failed, rolled back");
            }
        }
    }
}