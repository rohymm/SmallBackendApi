namespace SmallBackendApi.Models
{
    public sealed class Employee : BaseModel
    {
        
        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}