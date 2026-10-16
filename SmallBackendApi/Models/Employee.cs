namespace SmallBackendApi.Models
{
    public sealed class Employee : BaseModel
    {
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}