using System.ComponentModel.DataAnnotations;

namespace CBSESchool.Models
{
    public class Notice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(50)]
        public string TargetRole { get; set; } = "All"; // All, Students, Teachers, Parents

        public DateTime PostDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public bool IsImportant { get; set; } = false;

        [StringLength(100)]
        public string PostedBy { get; set; } = "Principal Office";
    }
}
