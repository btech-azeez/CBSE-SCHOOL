using CBSESchool.Models;
using CBSESchool.Repositories;

namespace CBSESchool.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _unitOfWork.Students.GetAllAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _unitOfWork.Students.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Student>> GetStudentsByClassIdAsync(int classId)
        {
            return await _unitOfWork.Students.FindAsync(s => s.ClassId == classId);
        }

        public async Task AddStudentAsync(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.RollNumber))
            {
                student.RollNumber = await GenerateRollNumberAsync(student.ClassId);
            }
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _unitOfWork.Students.Update(student);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student != null)
            {
                _unitOfWork.Students.Remove(student);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<string> GenerateRollNumberAsync(int classId)
        {
            var schoolClass = await _unitOfWork.Classes.GetByIdAsync(classId);
            string className = schoolClass != null ? schoolClass.ClassName.Replace("Class ", "").Replace(" ", "") : "GEN";
            var existingStudents = await _unitOfWork.Students.FindAsync(s => s.ClassId == classId);
            int nextCount = existingStudents.Count() + 1;
            return $"CBSE{DateTime.UtcNow.Year}-{className}-{nextCount:D2}";
        }
    }
}
