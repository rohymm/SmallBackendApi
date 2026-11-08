using SmallBackendApi.Models;

namespace SmallBackendApi.Services
{
    public interface IEmployeeService
    {
        // Get all employees with their department
        Task<List<Employee>> GetAllAsync();

        // Get one employee by id (null if not found)
        Task<Employee?> GetByIdAsync(int id);

        // Get employees that earn more than the minimum salary
        Task<List<Employee>> GetHighSalaryAsync();
    }
}