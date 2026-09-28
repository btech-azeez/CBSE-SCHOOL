using Xunit;
using Moq;
using CBSESchool.Models;
using CBSESchool.Repositories;
using CBSESchool.Services;

namespace CBSESchool.Tests
{
    public class StudentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Student>> _mockStudentRepo;
        private readonly Mock<IRepository<SchoolClass>> _mockClassRepo;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStudentRepo = new Mock<IRepository<Student>>();
            _mockClassRepo = new Mock<IRepository<SchoolClass>>();

            _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepo.Object);
            _mockUnitOfWork.Setup(u => u.Classes).Returns(_mockClassRepo.Object);

            _studentService = new StudentService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ReturnsAllStudents()
        {
            // Arrange
            var sampleStudents = new List<Student>
            {
                new Student { Id = 1, FullName = "Student A", ClassId = 1 },
                new Student { Id = 2, FullName = "Student B", ClassId = 1 }
            };
            _mockStudentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sampleStudents);

            // Act
            var result = await _studentService.GetAllStudentsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddStudentAsync_GeneratesRollNumberIfEmpty()
        {
            // Arrange
            var schoolClass = new SchoolClass { Id = 1, ClassName = "Class X", Section = "A" };
            _mockClassRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(schoolClass);
            _mockStudentRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>()))
                             .ReturnsAsync(new List<Student>());

            var newStudent = new Student { FullName = "Test Student", ClassId = 1, RollNumber = "" };

            // Act
            await _studentService.AddStudentAsync(newStudent);

            // Assert
            Assert.False(string.IsNullOrEmpty(newStudent.RollNumber));
            Assert.Contains("CBSE", newStudent.RollNumber);
            _mockStudentRepo.Verify(r => r.AddAsync(newStudent), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
