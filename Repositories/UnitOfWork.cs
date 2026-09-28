using CBSESchool.Data;
using CBSESchool.Models;

namespace CBSESchool.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<Student> Students { get; }
        public IRepository<Teacher> Teachers { get; }
        public IRepository<SchoolClass> Classes { get; }
        public IRepository<Subject> Subjects { get; }
        public IRepository<Exam> Exams { get; }
        public IRepository<Mark> Marks { get; }
        public IRepository<Notice> Notices { get; }
        public IRepository<FeeStructure> FeeStructures { get; }
        public IRepository<StudentFee> StudentFees { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Students = new Repository<Student>(_context);
            Teachers = new Repository<Teacher>(_context);
            Classes = new Repository<SchoolClass>(_context);
            Subjects = new Repository<Subject>(_context);
            Exams = new Repository<Exam>(_context);
            Marks = new Repository<Mark>(_context);
            Notices = new Repository<Notice>(_context);
            FeeStructures = new Repository<FeeStructure>(_context);
            StudentFees = new Repository<StudentFee>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
