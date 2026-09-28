using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBSESchool.Models
{
    public class StudentFee
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        public int FeeStructureId { get; set; }

        [ForeignKey(nameof(FeeStructureId))]
        public FeeStructure? FeeStructure { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string PaymentMode { get; set; } = "UPI / Online"; // Cash, Cheque, UPI, Online Bank Transfer

        [StringLength(50)]
        public string TransactionId { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Paid"; // Paid, Partial, Pending
    }
}
