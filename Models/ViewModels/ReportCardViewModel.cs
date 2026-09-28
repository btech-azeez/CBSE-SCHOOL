namespace CBSESchool.Models.ViewModels
{
    public class SubjectResultViewModel
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public double MarksObtained { get; set; }
        public double MaxMarks { get; set; } = 100;
        public string Grade { get; set; } = "A1";
        public string Remarks { get; set; } = string.Empty;
        public bool IsPassing { get; set; } = true;
    }

    public class ReportCardViewModel
    {
        public Student Student { get; set; } = new Student();
        public Exam Exam { get; set; } = new Exam();
        public SchoolClass SchoolClass { get; set; } = new SchoolClass();
        public List<SubjectResultViewModel> SubjectResults { get; set; } = new List<SubjectResultViewModel>();
        public double TotalMarksObtained { get; set; }
        public double TotalMaxMarks { get; set; }
        public double OverallPercentage { get; set; }
        public double CGPA { get; set; }
        public string OverallGrade { get; set; } = "A1";
        public string FinalResultStatus { get; set; } = "PASSED";
    }
}
