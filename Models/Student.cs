using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBSESchool.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string RollNumber { get; set; } = string.Empty; // CBSE Roll No format e.g. "CBSE2026-X-01"

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } = "Male";

        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public SchoolClass? SchoolClass { get; set; }

        [StringLength(100)]
        public string ParentName { get; set; } = string.Empty;

        [StringLength(20)]
        public string ParentContact { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string ParentEmail { get; set; } = string.Empty;

        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; } // Foreign Key to ApplicationUser (if linked)

        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
        public ICollection<StudentFee> Fees { get; set; } = new List<StudentFee>();
    }
}
