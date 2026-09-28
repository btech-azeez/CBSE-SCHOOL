namespace CBSESchool.Services
{
    public interface IGradeCalculatorService
    {
        string CalculateGrade(double marksObtained, double maxMarks = 100);
        string GetRemarks(string grade);
        bool IsPassing(double marksObtained, double passMarks = 33);
        double CalculateCGPA(IEnumerable<double> percentages);
    }
}
