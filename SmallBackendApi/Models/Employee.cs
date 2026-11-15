namespace SmallBackendApi.Models
{
    public sealed class Employee : BaseModel
    {
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        // New in day 6: unique email for each employee
        public string? Email { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // One employee has many tasks
        public List<EmployeeTask> Tasks { get; set; } = new();
    }
}