using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBSESchool.Models
{
    public class Mark
    {
        [Key]
        public int Id { get; set; }

        public int ExamId { get; set; }

        [ForeignKey(nameof(ExamId))]
        public Exam? Exam { get; set; }

        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        public int SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public Subject? Subject { get; set; }

        [Range(0, 100)]
        public double MarksObtained { get; set; }

        public double MaxMarks { get; set; } = 100;

        [StringLength(5)]
        public string Grade { get; set; } = "A1"; // CBSE Grade: A1, A2, B1, B2, C1, C2, D, E

        [StringLength(100)]
        public string Remarks { get; set; } = "Outstanding";
    }
}
