using CBSESchool.Models;

namespace CBSESchool.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<IEnumerable<Student>> GetStudentsByClassIdAsync(int classId);
        Task AddStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int id);
        Task<string> GenerateRollNumberAsync(int classId);
    }
}
