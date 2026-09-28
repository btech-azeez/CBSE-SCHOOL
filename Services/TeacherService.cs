using CBSESchool.Models;
using CBSESchool.Repositories;

namespace CBSESchool.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeacherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _unitOfWork.Teachers.GetAllAsync();
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _unitOfWork.Teachers.GetByIdAsync(id);
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            if (string.IsNullOrWhiteSpace(teacher.EmployeeCode))
            {
                var existing = await _unitOfWork.Teachers.GetAllAsync();
                teacher.EmployeeCode = $"EMP-T-{existing.Count() + 101}";
            }
            await _unitOfWork.Teachers.AddAsync(teacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher != null)
            {
                _unitOfWork.Teachers.Remove(teacher);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
