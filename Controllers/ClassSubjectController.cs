using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CBSESchool.Models;
using CBSESchool.Repositories;

namespace CBSESchool.Controllers
{
    public class ClassSubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClassSubjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            var subjects = await _unitOfWork.Subjects.GetAllAsync();

            ViewBag.Subjects = subjects;
            return View(classes);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateClass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateClass(SchoolClass schoolClass)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Classes.AddAsync(schoolClass);
                await _unitOfWork.CompleteAsync();
                TempData["SuccessMessage"] = $"Class '{schoolClass.ClassName} ({schoolClass.Section})' created.";
                return RedirectToAction(nameof(Index));
            }
            return View(schoolClass);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubject()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Subjects.AddAsync(subject);
                await _unitOfWork.CompleteAsync();
                TempData["SuccessMessage"] = $"CBSE Subject '{subject.SubjectName} ({subject.SubjectCode})' created.";
                return RedirectToAction(nameof(Index));
            }
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName", subject.ClassId);
            return View(subject);
        }
    }
}
