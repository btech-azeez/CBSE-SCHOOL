namespace CBSESchool.Services
{
    public class GradeCalculatorService : IGradeCalculatorService
    {
        public string CalculateGrade(double marksObtained, double maxMarks = 100)
        {
            if (maxMarks <= 0) return "F";
            double percentage = (marksObtained / maxMarks) * 100;

            return percentage switch
            {
                >= 91 => "A1",
                >= 81 => "A2",
                >= 71 => "B1",
                >= 61 => "B2",
                >= 51 => "C1",
                >= 41 => "C2",
                >= 33 => "D",
                _ => "E"
            };
        }

        public string GetRemarks(string grade)
        {
            return grade.ToUpper() switch
            {
                "A1" => "Outstanding Performance",
                "A2" => "Excellent Performance",
                "B1" => "Very Good Progress",
                "B2" => "Good Progress",
                "C1" => "Above Average Effort",
                "C2" => "Average - Scope for Improvement",
                "D" => "Marginal Pass - Needs Extra Attention",
                "E" => "Essential Repeat - Needs Special Coaching",
                _ => "N/A"
            };
        }

        public bool IsPassing(double marksObtained, double passMarks = 33)
        {
            return marksObtained >= passMarks;
        }

        public double CalculateCGPA(IEnumerable<double> percentages)
        {
            if (percentages == null || !percentages.Any()) return 0.0;
            double avgPercentage = percentages.Average();
            // CBSE CGPA calculation formula = Average Percentage / 9.5
            double cgpa = avgPercentage / 9.5;
            return Math.Min(10.0, Math.Round(cgpa, 2));
        }
    }
}
