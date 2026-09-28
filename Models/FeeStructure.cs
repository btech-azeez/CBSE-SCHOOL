using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBSESchool.Models
{
    public class FeeStructure
    {
        [Key]
        public int Id { get; set; }

        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public SchoolClass? SchoolClass { get; set; }

        [Required]
        [StringLength(50)]
        public string FeeType { get; set; } = "Tuition Fee"; // Tuition Fee, Lab Fee, Admission Fee, Exam Fee

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public string AcademicYear { get; set; } = "2025-2026";
    }
}
