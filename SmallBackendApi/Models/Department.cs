namespace SmallBackendApi.Models
{
    public sealed class Department : BaseModel
    {
        // One department has many employees
        public List<Employee> Employees { get; set; } = new();
    }
}