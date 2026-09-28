using Xunit;
using Moq;
using CBSESchool.Models;
using CBSESchool.Repositories;
using CBSESchool.Services;

namespace CBSESchool.Tests
{
    public class NoticeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Notice>> _mockNoticeRepo;
        private readonly NoticeService _noticeService;

        public NoticeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockNoticeRepo = new Mock<IRepository<Notice>>();

            _mockUnitOfWork.Setup(u => u.Notices).Returns(_mockNoticeRepo.Object);
            _noticeService = new NoticeService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllNoticesAsync_FiltersByTargetRoleForStudents()
        {
            // Arrange
            var notices = new List<Notice>
            {
                new Notice { Id = 1, Title = "General Notice", TargetRole = "All", PostDate = DateTime.UtcNow },
                new Notice { Id = 2, Title = "Student Circular", TargetRole = "Students", PostDate = DateTime.UtcNow },
                new Notice { Id = 3, Title = "Staff Meeting", TargetRole = "Teachers", PostDate = DateTime.UtcNow }
            };
            _mockNoticeRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(notices);

            // Act
            var result = await _noticeService.GetAllNoticesAsync("Students");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.DoesNotContain(result, n => n.TargetRole == "Teachers");
        }
    }
}
