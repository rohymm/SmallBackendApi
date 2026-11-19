using Microsoft.AspNetCore.Mvc;
using SmallBackendApi.Services;

namespace SmallBackendApi.Controllers
{
    [Route("api/service/employees")]
    [ApiController]
    public class ServiceEmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        // Constructor injection: we ask for the interface, not the class
        public ServiceEmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpGet("high-salary")]
        public async Task<IActionResult> GetHighSalary()
        {
            var employees = await _employeeService.GetHighSalaryAsync();
            return Ok(employees);
        }
    }
}