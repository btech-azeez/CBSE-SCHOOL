using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CBSESchool.Models;
using CBSESchool.Services;
using CBSESchool.Repositories;

namespace CBSESchool.Controllers
{
    public class FeeController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;

        public FeeController(IFeeService feeService, IStudentService studentService, IUnitOfWork unitOfWork)
        {
            _feeService = feeService;
            _studentService = studentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var feeStructures = await _feeService.GetFeeStructuresAsync();
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.Classes = classes.ToDictionary(c => c.Id, c => c.ClassName);
            return View(feeStructures);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFeeStructure()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFeeStructure(FeeStructure feeStructure)
        {
            if (ModelState.IsValid)
            {
                await _feeService.AddFeeStructureAsync(feeStructure);
                TempData["SuccessMessage"] = "Fee structure added successfully.";
                return RedirectToAction(nameof(Index));
            }
            var classes = await _unitOfWork.Classes.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "ClassName", feeStructure.ClassId);
            return View(feeStructure);
        }

        [Authorize(Roles = "Admin,Teacher,Student,Parent")]
        public async Task<IActionResult> PayFee(int? studentId)
        {
            var students = await _studentService.GetAllStudentsAsync();
            var feeStructures = await _feeService.GetFeeStructuresAsync();

            ViewBag.Students = new SelectList(students, "Id", "FullName", studentId);
            ViewBag.FeeStructures = new SelectList(feeStructures, "Id", "FeeType");

            var studentFee = new StudentFee
            {
                StudentId = studentId ?? (students.FirstOrDefault()?.Id ?? 0),
                AmountPaid = 15000.00m,
                PaymentMode = "Online UPI"
            };

            return View(studentFee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher,Student,Parent")]
        public async Task<IActionResult> PayFee(StudentFee studentFee)
        {
            if (ModelState.IsValid)
            {
                await _feeService.RecordFeePaymentAsync(studentFee);
                TempData["SuccessMessage"] = $"Fee payment recorded successfully! Transaction ID: {studentFee.TransactionId}";
                return RedirectToAction(nameof(Index));
            }
            var students = await _studentService.GetAllStudentsAsync();
            var feeStructures = await _feeService.GetFeeStructuresAsync();

            ViewBag.Students = new SelectList(students, "Id", "FullName", studentFee.StudentId);
            ViewBag.FeeStructures = new SelectList(feeStructures, "Id", "FeeType", studentFee.FeeStructureId);
            return View(studentFee);
        }
    }
}
