using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Models;

namespace SmallBackendApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployeeService> _logger;

        // All three objects are injected by the framework
        public EmployeeService(AppDbContext context, IConfiguration configuration, ILogger<EmployeeService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            _logger.LogInformation("Loading all employees");

            return await _context.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                // Warning: something unexpected, but the app keeps working
                _logger.LogWarning("Employee with id {EmployeeId} was not found", id);
            }

            return employee;
        }

        public async Task<List<Employee>> GetHighSalaryAsync()
        {
            // Read the business setting from appsettings.json
            decimal minSalary = _configuration.GetValue<decimal>("BusinessSettings:MinHighSalary");

            _logger.LogInformation("Loading employees with salary above {MinSalary}", minSalary);

            return await _context.Employees
                .Where(e => e.Salary > minSalary)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}