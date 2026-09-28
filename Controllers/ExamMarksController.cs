using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CBSESchool.Models;
using CBSESchool.Models.ViewModels;
using CBSESchool.Services;
using CBSESchool.Repositories;

namespace CBSESchool.Controllers
{
    public class ExamMarksController : Controller
    {
        private readonly IExamService _examService;
        private readonly IStudentService _studentService;
        private readonly IGradeCalculatorService _gradeCalculator;
        private readonly IUnitOfWork _unitOfWork;

        public ExamMarksController(
            IExamService examService,
            IStudentService studentService,
            IGradeCalculatorService gradeCalculator,
            IUnitOfWork unitOfWork)
        {
            _examService = examService;
            _studentService = studentService;
            _gradeCalculator = gradeCalculator;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _examService.GetAllExamsAsync();
            var classes = await _unitOfWork.Classes.GetAllAsync();

            ViewBag.Classes = classes.ToDictionary(c => c.Id, c => $"{c.ClassName} ({c.Section})");
            return View(exams);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CreateExam()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CreateExam(Exam exam)
        {
            if (ModelState.IsValid)
            {
                await _examService.AddExamAsync(exam);
                TempData["SuccessMessage"] = $"Exam '{exam.ExamName}' scheduled successfully.";
                return RedirectToAction(nameof(Index));
            }
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName", exam.ClassId);
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> EnterMarks(int examId, int? studentId)
        {
            var exam = await _examService.GetExamByIdAsync(examId);
            if (exam == null) return NotFound();

            var students = await _studentService.GetStudentsByClassIdAsync(exam.ClassId);
            var subjects = await _unitOfWork.Subjects.FindAsync(s => s.ClassId == exam.ClassId);

            ViewBag.Exam = exam;
            ViewBag.Students = new SelectList(students, "Id", "FullName", studentId);
            ViewBag.Subjects = new SelectList(subjects, "Id", "SubjectName");

            var markModel = new Mark
            {
                ExamId = examId,
                StudentId = studentId ?? (students.FirstOrDefault()?.Id ?? 0),
                MaxMarks = 100
            };

            return View(markModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> EnterMarks(Mark mark)
        {
            if (ModelState.IsValid)
            {
                await _examService.SaveMarkAsync(mark);
                TempData["SuccessMessage"] = "Marks recorded & CBSE Grade calculated!";
                return RedirectToAction(nameof(EnterMarks), new { examId = mark.ExamId, studentId = mark.StudentId });
            }
            var exam = await _examService.GetExamByIdAsync(mark.ExamId);
            int classId = exam?.ClassId ?? 0;
            var students = await _studentService.GetStudentsByClassIdAsync(classId);
            var subjects = await _unitOfWork.Subjects.FindAsync(s => s.ClassId == classId);

            ViewBag.Exam = exam;
            ViewBag.Students = new SelectList(students, "Id", "FullName", mark.StudentId);
            ViewBag.Subjects = new SelectList(subjects, "Id", "SubjectName", mark.SubjectId);
            return View(mark);
        }

        public async Task<IActionResult> ReportCard(int studentId, int examId)
        {
            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null) return NotFound();

            var exam = await _examService.GetExamByIdAsync(examId);
            if (exam == null) return NotFound();

            var schoolClass = await _unitOfWork.Classes.GetByIdAsync(student.ClassId);
            var marks = await _examService.GetMarksForStudentAsync(studentId, examId);
            var subjects = await _unitOfWork.Subjects.FindAsync(s => s.ClassId == student.ClassId);

            var subjectResults = new List<SubjectResultViewModel>();
            foreach (var subject in subjects)
            {
                var m = marks.FirstOrDefault(x => x.SubjectId == subject.Id);
                double obtained = m?.MarksObtained ?? 0;
                double max = m?.MaxMarks ?? subject.MaxMarks;
                string grade = m?.Grade ?? _gradeCalculator.CalculateGrade(obtained, max);
                string remarks = m?.Remarks ?? _gradeCalculator.GetRemarks(grade);

                subjectResults.Add(new SubjectResultViewModel
                {
                    SubjectCode = subject.SubjectCode,
                    SubjectName = subject.SubjectName,
                    MarksObtained = obtained,
                    MaxMarks = max,
                    Grade = grade,
                    Remarks = remarks,
                    IsPassing = _gradeCalculator.IsPassing(obtained, subject.PassMarks)
                });
            }

            double totalObtained = subjectResults.Sum(s => s.MarksObtained);
            double totalMax = subjectResults.Sum(s => s.MaxMarks);
            double overallPercentage = totalMax > 0 ? (totalObtained / totalMax) * 100 : 0;
            double cgpa = _gradeCalculator.CalculateCGPA(subjectResults.Select(s => (s.MarksObtained / s.MaxMarks) * 100));

            bool failedAny = subjectResults.Any(s => !s.IsPassing);

            var reportCard = new ReportCardViewModel
            {
                Student = student,
                Exam = exam,
                SchoolClass = schoolClass ?? new SchoolClass(),
                SubjectResults = subjectResults,
                TotalMarksObtained = totalObtained,
                TotalMaxMarks = totalMax,
                OverallPercentage = Math.Round(overallPercentage, 2),
                CGPA = cgpa,
                OverallGrade = _gradeCalculator.CalculateGrade(overallPercentage, 100),
                FinalResultStatus = failedAny ? "ESSENTIAL REPEAT (COMPARTMENT)" : "PASSED - QUALIFIED FOR ADMISSION TO NEXT CLASS"
            };

            return View(reportCard);
        }

        // AJAX API Endpoint for Real-time Grade Calculation
        [HttpGet]
        public IActionResult CalculateGradeJson(double marksObtained, double maxMarks = 100)
        {
            string grade = _gradeCalculator.CalculateGrade(marksObtained, maxMarks);
            string remarks = _gradeCalculator.GetRemarks(grade);
            double percentage = maxMarks > 0 ? Math.Round((marksObtained / maxMarks) * 100, 2) : 0;
            bool isPassing = _gradeCalculator.IsPassing(marksObtained, 33);

            return Json(new
            {
                success = true,
                grade = grade,
                remarks = remarks,
                percentage = percentage,
                isPassing = isPassing
            });
        }
    }
}
