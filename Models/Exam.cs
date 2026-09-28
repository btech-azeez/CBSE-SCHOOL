using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBSESchool.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ExamName { get; set; } = string.Empty; // e.g. "Unit Test 1", "Half Yearly Exam", "Annual CBSE Pre-Board"

        [Required]
        [StringLength(20)]
        public string AcademicYear { get; set; } = "2025-2026";

        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public SchoolClass? SchoolClass { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExamDate { get; set; } = DateTime.UtcNow;

        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
    }
}
