using System.ComponentModel.DataAnnotations;

namespace CBSESchool.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string EmployeeCode { get; set; } = string.Empty; // e.g., "EMP-T-101"

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        public string Qualification { get; set; } = "M.Sc, B.Ed";

        [StringLength(100)]
        public string SpecializationSubject { get; set; } = "Mathematics";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; } // Foreign Key to ApplicationUser
    }
}
