using CBSESchool.Models;

namespace CBSESchool.Services
{
    public interface IFeeService
    {
        Task<IEnumerable<FeeStructure>> GetFeeStructuresAsync();
        Task<FeeStructure?> GetFeeStructureByIdAsync(int id);
        Task AddFeeStructureAsync(FeeStructure feeStructure);
        Task<IEnumerable<StudentFee>> GetStudentFeePaymentsAsync(int studentId);
        Task RecordFeePaymentAsync(StudentFee studentFee);
        Task<decimal> GetTotalFeesPaidByStudentAsync(int studentId);
    }
}
