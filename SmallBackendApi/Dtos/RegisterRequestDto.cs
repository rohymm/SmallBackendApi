using System.ComponentModel.DataAnnotations;

namespace SmallBackendApi.Dtos
{
    public class RegisterRequestDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        // Training only: lets us pick a role while testing.
        // In production, only an Admin could assign roles.
        public string Role { get; set; } = "Employee";
    }
}