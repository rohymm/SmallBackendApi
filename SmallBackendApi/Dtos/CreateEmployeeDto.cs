using System.ComponentModel.DataAnnotations;

namespace SmallBackendApi.Dtos
{
    public class CreateEmployeeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(1000, 1000000)]
        public decimal Salary { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required")]
        public int DepartmentId { get; set; }
    }
}