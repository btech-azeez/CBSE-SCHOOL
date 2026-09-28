using Xunit;
using Moq;
using CBSESchool.Models;
using CBSESchool.Repositories;
using CBSESchool.Services;

namespace CBSESchool.Tests
{
    public class ExamServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Exam>> _mockExamRepo;
        private readonly Mock<IRepository<Mark>> _mockMarkRepo;
        private readonly Mock<IGradeCalculatorService> _mockGradeCalculator;
        private readonly ExamService _examService;

        public ExamServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockExamRepo = new Mock<IRepository<Exam>>();
            _mockMarkRepo = new Mock<IRepository<Mark>>();
            _mockGradeCalculator = new Mock<IGradeCalculatorService>();

            _mockUnitOfWork.Setup(u => u.Exams).Returns(_mockExamRepo.Object);
            _mockUnitOfWork.Setup(u => u.Marks).Returns(_mockMarkRepo.Object);

            _examService = new ExamService(_mockUnitOfWork.Object, _mockGradeCalculator.Object);
        }

        [Fact]
        public async Task SaveMarkAsync_CalculatesGradeAndSavesMark()
        {
            // Arrange
            var mark = new Mark { ExamId = 1, StudentId = 1, SubjectId = 1, MarksObtained = 92, MaxMarks = 100 };
            _mockGradeCalculator.Setup(g => g.CalculateGrade(92, 100)).Returns("A1");
            _mockGradeCalculator.Setup(g => g.GetRemarks("A1")).Returns("Outstanding");

            _mockMarkRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Mark, bool>>>()))
                         .ReturnsAsync(new List<Mark>());

            // Act
            await _examService.SaveMarkAsync(mark);

            // Assert
            Assert.Equal("A1", mark.Grade);
            Assert.Equal("Outstanding", mark.Remarks);
            _mockMarkRepo.Verify(r => r.AddAsync(mark), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
