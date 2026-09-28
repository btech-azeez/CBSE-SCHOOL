using CBSESchool.Models;
using CBSESchool.Repositories;

namespace CBSESchool.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGradeCalculatorService _gradeCalculator;

        public ExamService(IUnitOfWork unitOfWork, IGradeCalculatorService gradeCalculator)
        {
            _unitOfWork = unitOfWork;
            _gradeCalculator = gradeCalculator;
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _unitOfWork.Exams.GetAllAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(int id)
        {
            return await _unitOfWork.Exams.GetByIdAsync(id);
        }

        public async Task AddExamAsync(Exam exam)
        {
            await _unitOfWork.Exams.AddAsync(exam);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateExamAsync(Exam exam)
        {
            _unitOfWork.Exams.Update(exam);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteExamAsync(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam != null)
            {
                _unitOfWork.Exams.Remove(exam);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Mark>> GetMarksForStudentAsync(int studentId, int examId)
        {
            return await _unitOfWork.Marks.FindAsync(m => m.StudentId == studentId && m.ExamId == examId);
        }

        public async Task SaveMarkAsync(Mark mark)
        {
            mark.Grade = _gradeCalculator.CalculateGrade(mark.MarksObtained, mark.MaxMarks);
            mark.Remarks = _gradeCalculator.GetRemarks(mark.Grade);

            var existing = (await _unitOfWork.Marks.FindAsync(m => m.ExamId == mark.ExamId && m.StudentId == mark.StudentId && m.SubjectId == mark.SubjectId)).FirstOrDefault();

            if (existing != null)
            {
                existing.MarksObtained = mark.MarksObtained;
                existing.MaxMarks = mark.MaxMarks;
                existing.Grade = mark.Grade;
                existing.Remarks = mark.Remarks;
                _unitOfWork.Marks.Update(existing);
            }
            else
            {
                await _unitOfWork.Marks.AddAsync(mark);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
