using CBSESchool.Models;

namespace CBSESchool.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Student> Students { get; }
        IRepository<Teacher> Teachers { get; }
        IRepository<SchoolClass> Classes { get; }
        IRepository<Subject> Subjects { get; }
        IRepository<Exam> Exams { get; }
        IRepository<Mark> Marks { get; }
        IRepository<Notice> Notices { get; }
        IRepository<FeeStructure> FeeStructures { get; }
        IRepository<StudentFee> StudentFees { get; }

        Task<int> CompleteAsync();
    }
}
