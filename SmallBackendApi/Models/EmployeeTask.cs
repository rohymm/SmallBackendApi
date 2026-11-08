namespace SmallBackendApi.Models
{
    public sealed class EmployeeTask
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // False = the task is still incomplete
        public bool IsCompleted { get; set; }

        // Foreign key to Employee
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}