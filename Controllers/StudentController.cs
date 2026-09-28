using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CBSESchool.Models;
using CBSESchool.Services;
using CBSESchool.Repositories;

namespace CBSESchool.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;

        public StudentController(IStudentService studentService, IUnitOfWork unitOfWork)
        {
            _studentService = studentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int? classId, string? searchString)
        {
            var students = await _studentService.GetAllStudentsAsync();
            var classes = await _unitOfWork.Classes.GetAllAsync();

            if (classId.HasValue && classId.Value > 0)
            {
                students = students.Where(s => s.ClassId == classId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                students = students.Where(s => s.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               s.RollNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Classes = new SelectList(classes, "Id", "ClassName", classId);
            ViewBag.SelectedClassId = classId;
            ViewBag.SearchString = searchString;

            return View(students);
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();

            student.SchoolClass = await _unitOfWork.Classes.GetByIdAsync(student.ClassId);
            return View(student);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                await _studentService.AddStudentAsync(student);
                TempData["SuccessMessage"] = $"Student '{student.FullName}' enrolled successfully with Roll No {student.RollNumber}.";
                return RedirectToAction(nameof(Index));
            }
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName", student.ClassId);
            return View(student);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();

            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName", student.ClassId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _studentService.UpdateStudentAsync(student);
                TempData["SuccessMessage"] = $"Student details for '{student.FullName}' updated.";
                return RedirectToAction(nameof(Index));
            }
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName", student.ClassId);
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();
            student.SchoolClass = await _unitOfWork.Classes.GetByIdAsync(student.ClassId);
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            TempData["SuccessMessage"] = "Student record deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // AJAX API ENDPOINTS (Asynchronous Requests)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetStudentDetailsJson(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return Json(new { success = false, message = "Student not found" });

            var schoolClass = await _unitOfWork.Classes.GetByIdAsync(student.ClassId);

            return Json(new
            {
                success = true,
                id = student.Id,
                fullName = student.FullName,
                rollNumber = student.RollNumber,
                className = schoolClass != null ? $"{schoolClass.ClassName} ({schoolClass.Section})" : "N/A",
                gender = student.Gender,
                parentName = student.ParentName,
                parentContact = student.ParentContact,
                parentEmail = student.ParentEmail,
                address = student.Address,
                dob = student.DateOfBirth.ToString("dd MMM yyyy")
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectsByClassJson(int classId)
        {
            var subjects = await _unitOfWork.Subjects.FindAsync(s => s.ClassId == classId);
            var result = subjects.Select(s => new
            {
                id = s.Id,
                subjectCode = s.SubjectCode,
                subjectName = s.SubjectName,
                passMarks = s.PassMarks,
                maxMarks = s.MaxMarks
            });
            return Json(result);
        }
    }
}
