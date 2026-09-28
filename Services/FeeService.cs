using CBSESchool.Models;
using CBSESchool.Repositories;

namespace CBSESchool.Services
{
    public class FeeService : IFeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FeeStructure>> GetFeeStructuresAsync()
        {
            return await _unitOfWork.FeeStructures.GetAllAsync();
        }

        public async Task<FeeStructure?> GetFeeStructureByIdAsync(int id)
        {
            return await _unitOfWork.FeeStructures.GetByIdAsync(id);
        }

        public async Task AddFeeStructureAsync(FeeStructure feeStructure)
        {
            await _unitOfWork.FeeStructures.AddAsync(feeStructure);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<StudentFee>> GetStudentFeePaymentsAsync(int studentId)
        {
            return await _unitOfWork.StudentFees.FindAsync(f => f.StudentId == studentId);
        }

        public async Task RecordFeePaymentAsync(StudentFee studentFee)
        {
            if (string.IsNullOrWhiteSpace(studentFee.TransactionId))
            {
                studentFee.TransactionId = $"TXN-CBSE-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            }
            studentFee.PaymentDate = DateTime.UtcNow;
            await _unitOfWork.StudentFees.AddAsync(studentFee);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<decimal> GetTotalFeesPaidByStudentAsync(int studentId)
        {
            var payments = await _unitOfWork.StudentFees.FindAsync(f => f.StudentId == studentId && f.Status == "Paid");
            return payments.Sum(p => p.AmountPaid);
        }
    }
}
