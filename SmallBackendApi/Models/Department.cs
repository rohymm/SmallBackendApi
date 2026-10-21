namespace SmallBackendApi.Models
{

    public sealed class Department : BaseModel
    {
        
        public List<Employee> Employees { get; set; }
    }
}