namespace SmallBackendApi.Models
{
    public sealed class Employee : BaseModel
    {
        public decimal Salary { get; set; }

        // New property added for the review challenge
        public DateTime HireDate { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // One employee has many tasks
        public List<EmployeeTask> Tasks { get; set; } = new();
    }
}