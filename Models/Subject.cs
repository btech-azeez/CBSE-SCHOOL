using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBSESchool.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string SubjectCode { get; set; } = string.Empty; // e.g. "041", "042", "301"

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; } = string.Empty; // e.g., "Mathematics", "Physics", "English Core"

        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public SchoolClass? SchoolClass { get; set; }

        public int PassMarks { get; set; } = 33;
        public int MaxMarks { get; set; } = 100;
    }
}
