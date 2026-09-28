using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CBSESchool.Models;
using CBSESchool.Services;

namespace CBSESchool.Controllers
{
    public class NoticeController : Controller
    {
        private readonly INoticeService _noticeService;

        public NoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        public async Task<IActionResult> Index()
        {
            string role = User.IsInRole("Admin") ? "Admin" :
                         User.IsInRole("Teacher") ? "Teachers" :
                         User.IsInRole("Student") ? "Students" : "All";

            var notices = await _noticeService.GetAllNoticesAsync(role);
            return View(notices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var notice = await _noticeService.GetNoticeByIdAsync(id);
            if (notice == null) return NotFound();
            return View(notice);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create(Notice notice)
        {
            if (ModelState.IsValid)
            {
                notice.PostDate = DateTime.UtcNow;
                if (string.IsNullOrWhiteSpace(notice.PostedBy))
                {
                    notice.PostedBy = User.Identity?.Name ?? "School Administration";
                }
                await _noticeService.AddNoticeAsync(notice);
                TempData["SuccessMessage"] = "Notice posted successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(notice);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var notice = await _noticeService.GetNoticeByIdAsync(id);
            if (notice == null) return NotFound();
            return View(notice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _noticeService.DeleteNoticeAsync(id);
            TempData["SuccessMessage"] = "Notice deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
