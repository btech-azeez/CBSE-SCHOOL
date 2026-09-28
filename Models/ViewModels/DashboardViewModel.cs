namespace CBSESchool.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public int ActiveNoticesCount { get; set; }
        public decimal TotalFeeCollection { get; set; }
        public List<Notice> RecentNotices { get; set; } = new List<Notice>();
        public List<Student> RecentEnrolledStudents { get; set; } = new List<Student>();
    }
}
