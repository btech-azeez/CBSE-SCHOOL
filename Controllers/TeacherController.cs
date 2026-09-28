using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CBSESchool.Models;
using CBSESchool.Services;

namespace CBSESchool.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return View(teachers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                await _teacherService.AddTeacherAsync(teacher);
                TempData["SuccessMessage"] = $"Teacher '{teacher.FullName}' added with Employee Code {teacher.EmployeeCode}.";
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _teacherService.UpdateTeacherAsync(teacher);
                TempData["SuccessMessage"] = $"Teacher record for '{teacher.FullName}' updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teacherService.DeleteTeacherAsync(id);
            TempData["SuccessMessage"] = "Teacher record removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
