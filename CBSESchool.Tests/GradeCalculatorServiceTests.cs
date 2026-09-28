using Xunit;
using CBSESchool.Services;

namespace CBSESchool.Tests
{
    public class GradeCalculatorServiceTests
    {
        private readonly GradeCalculatorService _gradeCalculator;

        public GradeCalculatorServiceTests()
        {
            _gradeCalculator = new GradeCalculatorService();
        }

        [Theory]
        [InlineData(95, 100, "A1")]
        [InlineData(91, 100, "A1")]
        [InlineData(85, 100, "A2")]
        [InlineData(75, 100, "B1")]
        [InlineData(65, 100, "B2")]
        [InlineData(55, 100, "C1")]
        [InlineData(45, 100, "C2")]
        [InlineData(35, 100, "D")]
        [InlineData(25, 100, "E")]
        public void CalculateGrade_ReturnsCorrectCBSEGrade(double obtained, double maxMarks, string expectedGrade)
        {
            // Act
            string result = _gradeCalculator.CalculateGrade(obtained, maxMarks);

            // Assert
            Assert.Equal(expectedGrade, result);
        }

        [Fact]
        [Trait("Category", "Grading")]
        public void CalculateCGPA_ValidPercentages_ReturnsCorrectValue()
        {
            // Arrange: Percentages for 5 subjects averaging 95%
            var percentages = new List<double> { 95.0, 95.0, 95.0, 95.0, 95.0 };

            // Act: 95 / 9.5 = 10.0
            double cgpa = _gradeCalculator.CalculateCGPA(percentages);

            // Assert
            Assert.Equal(10.0, cgpa);
        }

        [Theory]
        [InlineData(33, 33, true)]
        [InlineData(32.5, 33, false)]
        [InlineData(80, 33, true)]
        public void IsPassing_EvaluatesCorrectly(double marks, double passMarks, bool expectedResult)
        {
            // Act
            bool result = _gradeCalculator.IsPassing(marks, passMarks);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
