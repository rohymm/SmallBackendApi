namespace SmallBackendApi.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}