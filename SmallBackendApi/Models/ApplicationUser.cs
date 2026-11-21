using Microsoft.AspNetCore.Identity;

namespace SmallBackendApi.Models
{
    // IdentityUser already has Id, UserName, Email, PasswordHash (hashed, never plain text)...
    public class ApplicationUser : IdentityUser
    {
        // Extra field of our own
        public string? FullName { get; set; }
    }
}