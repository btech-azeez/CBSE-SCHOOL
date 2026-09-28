using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CBSESchool.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(50)]
        public string RoleName { get; set; } = "Student"; // Admin, Teacher, Student, Parent

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
