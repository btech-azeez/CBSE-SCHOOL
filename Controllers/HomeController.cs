using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CBSESchool.Models;
using CBSESchool.Models.ViewModels;
using CBSESchool.Services;
using CBSESchool.Repositories;

namespace CBSESchool.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly INoticeService _noticeService;
        private readonly IFeeService _feeService;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(
            IStudentService studentService,
            ITeacherService teacherService,
            INoticeService noticeService,
            IFeeService feeService,
            IUnitOfWork unitOfWork)
        {
            _studentService = studentService;
            _teacherService = teacherService;
            _noticeService = noticeService;
            _feeService = feeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var teachers = await _teacherService.GetAllTeachersAsync();
            var classes = await _unitOfWork.Classes.GetAllAsync();
            var notices = await _noticeService.GetAllNoticesAsync();
            var fees = await _feeService.GetFeeStructuresAsync();

            var viewModel = new DashboardViewModel
            {
                TotalStudents = students.Count(),
                TotalTeachers = teachers.Count(),
                TotalClasses = classes.Count(),
                ActiveNoticesCount = notices.Count(),
                RecentNotices = notices.Take(5).ToList(),
                RecentEnrolledStudents = students.OrderByDescending(s => s.EnrollmentDate).Take(5).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Academics()
        {
            return View();
        }

        public IActionResult Facilities()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
