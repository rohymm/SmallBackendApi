using System.ComponentModel.DataAnnotations;

namespace SmallBackendApi.Dtos
{
    public class EmployeeQueryParams
    {
        public int? DepartmentId { get; set; }

        // Search in name and email
        public string? Search { get; set; }

        // Allowed values: name, salary, hiredate
        public string SortBy { get; set; } = "name";

        // asc or desc
        public string SortDir { get; set; } = "asc";

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        // Bounded page size so nobody can ask for 1,000,000 rows
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}