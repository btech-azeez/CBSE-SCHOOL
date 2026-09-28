using CBSESchool.Models;

namespace CBSESchool.Services
{
    public interface IExamService
    {
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<Exam?> GetExamByIdAsync(int id);
        Task AddExamAsync(Exam exam);
        Task UpdateExamAsync(Exam exam);
        Task DeleteExamAsync(int id);
        Task<IEnumerable<Mark>> GetMarksForStudentAsync(int studentId, int examId);
        Task SaveMarkAsync(Mark mark);
    }
}
