using System.ComponentModel.DataAnnotations;

namespace SmallBackendApi.ViewModels
{
    public class EditEmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Range(1000, 1000000, ErrorMessage = "Salary must be between 1000 and 1,000,000")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Please choose a department")]
        public int DepartmentId { get; set; }
    }
}