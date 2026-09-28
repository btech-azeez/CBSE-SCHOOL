using System.ComponentModel.DataAnnotations;

namespace CBSESchool.Models
{
    public class SchoolClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ClassName { get; set; } = string.Empty; // e.g., "Class X", "Class XII"

        [Required]
        [StringLength(10)]
        public string Section { get; set; } = "A"; // e.g., "A", "B", "C"

        [StringLength(50)]
        public string Stream { get; set; } = "General"; // Science, Commerce, Arts, General

        public int Capacity { get; set; } = 40;

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
