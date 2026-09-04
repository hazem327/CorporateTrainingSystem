using CorporateTrainingSystem.Application.Features.Enrollments.CancelEnrollment;
using CorporateTrainingSystem.Application.Features.Enrollments.EnrollEmployee;
using CorporateTrainingSystem.Application.Features.Enrollments.GetTrainingHistory;
using CorporateTrainingSystem.Application.Features.Enrollments.ListEnrollments;
using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CorporateTrainingSystem.Web.Features.Enrollments
{
    [Authorize]
    public class EnrollmentsController : Controller
    {
        private readonly EnrollEmployeeHandler _enrollHandler;
        private readonly CancelEnrollmentHandler _cancelHandler;
        private readonly ListEnrollmentsHandler _listHandler;
        private readonly GetTrainingHistoryHandler _historyHandler;
        private readonly IValidator<EnrollEmployeeCommand> _enrollValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentsController(
            EnrollEmployeeHandler enrollHandler,
            CancelEnrollmentHandler cancelHandler,
            ListEnrollmentsHandler listHandler,
            GetTrainingHistoryHandler historyHandler,
            IValidator<EnrollEmployeeCommand> enrollValidator,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _enrollHandler = enrollHandler;
            _cancelHandler = cancelHandler;
            _listHandler = listHandler;
            _historyHandler = historyHandler;
            _enrollValidator = enrollValidator;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? sessionId, int? employeeId, EnrollmentStatus? status)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isManagerOrAdmin = User.IsInRole("Administrator") || User.IsInRole("TrainingManager");

            var query = new ListEnrollmentsQuery
            {
                SessionId = sessionId,
                Status = status
            };

            if (!isManagerOrAdmin)
            {
                // Employees can only view their own enrollments
                if (user?.EmployeeId == null)
                {
                    TempData["Error"] = "Your user account is not associated with an employee record.";
                    return View(new List<EnrollmentListItem>());
                }
                query.EmployeeId = user.EmployeeId;
            }
            else
            {
                query.EmployeeId = employeeId;
                await LoadFilterDropdowns(sessionId, employeeId);
            }

            var enrollments = await _listHandler.HandleAsync(query);
            return View(enrollments);
        }

        [Authorize(Roles = "Administrator,TrainingManager,Employee")]
        [HttpGet]
        public async Task<IActionResult> Create(int? sessionId)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isManagerOrAdmin = User.IsInRole("Administrator") || User.IsInRole("TrainingManager");

            var command = new EnrollEmployeeCommand();

            if (sessionId.HasValue)
            {
                command.TrainingSessionId = sessionId.Value;
            }

            if (!isManagerOrAdmin)
            {
                if (user?.EmployeeId == null)
                {
                    TempData["Error"] = "Your user account is not linked to an employee profile.";
                    return RedirectToAction(nameof(Index));
                }
                command.EmployeeId = user.EmployeeId.Value;
                var currentEmployee = await _unitOfWork.Repository<Employee>().GetByIdAsync(user.EmployeeId.Value);
                ViewBag.CurrentEmployeeName = currentEmployee?.FullName ?? "Current Employee";
                ViewBag.IsSelfEnrollment = true;
            }
            else
            {
                ViewBag.IsSelfEnrollment = false;
                await LoadEmployeesDropdown();
            }

            await LoadAvailableSessionsDropdown(command.TrainingSessionId);
            return View(command);
        }

        [Authorize(Roles = "Administrator,TrainingManager,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrollEmployeeCommand command)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isManagerOrAdmin = User.IsInRole("Administrator") || User.IsInRole("TrainingManager");

            if (!isManagerOrAdmin)
            {
                if (user?.EmployeeId == null)
                {
                    TempData["Error"] = "Your user account is not linked to an employee profile.";
                    return RedirectToAction(nameof(Index));
                }
                // Enforce self-enrollment for employee role
                command.EmployeeId = user.EmployeeId.Value;
                ViewBag.IsSelfEnrollment = true;
                var currentEmployee = await _unitOfWork.Repository<Employee>().GetByIdAsync(user.EmployeeId.Value);
                ViewBag.CurrentEmployeeName = currentEmployee?.FullName ?? "Current Employee";
            }
            else
            {
                ViewBag.IsSelfEnrollment = false;
                await LoadEmployeesDropdown(command.EmployeeId);
            }

            var validation = await _enrollValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                await LoadAvailableSessionsDropdown(command.TrainingSessionId);
                return View(command);
            }

            var result = await _enrollHandler.HandleAsync(command);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Enrollment failed.");
                await LoadAvailableSessionsDropdown(command.TrainingSessionId);
                return View(command);
            }

            TempData["Success"] = "Enrollment completed successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,TrainingManager,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isManagerOrAdmin = User.IsInRole("Administrator") || User.IsInRole("TrainingManager");

            var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(id);
            if (enrollment == null)
            {
                TempData["Error"] = "Enrollment not found.";
                return RedirectToAction(nameof(Index));
            }

            // An employee can only cancel their own enrollment
            if (!isManagerOrAdmin)
            {
                if (user?.EmployeeId == null || enrollment.EmployeeId != user.EmployeeId.Value)
                {
                    return Forbid();
                }
            }

            var result = await _cancelHandler.HandleAsync(new CancelEnrollmentCommand { EnrollmentId = id });
            if (result.Success)
            {
                TempData["Success"] = "Enrollment cancelled successfully (BR-07).";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> History(int? employeeId)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isManagerOrAdmin = User.IsInRole("Administrator") || User.IsInRole("TrainingManager");

            int targetEmployeeId;

            if (!isManagerOrAdmin)
            {
                if (user?.EmployeeId == null)
                {
                    TempData["Error"] = "Your user account is not linked to an employee profile.";
                    return RedirectToAction("Index", "Home");
                }
                targetEmployeeId = user.EmployeeId.Value;
            }
            else
            {
                if (employeeId.HasValue)
                {
                    targetEmployeeId = employeeId.Value;
                }
                else if (user?.EmployeeId.HasValue == true)
                {
                    targetEmployeeId = user.EmployeeId.Value;
                }
                else
                {
                    var firstEmp = (await _unitOfWork.Repository<Employee>().GetAllAsync()).FirstOrDefault(e => e.IsActive);
                    if (firstEmp == null)
                    {
                        TempData["Error"] = "No active employees found.";
                        return RedirectToAction(nameof(Index));
                    }
                    targetEmployeeId = firstEmp.Id;
                }

                await LoadEmployeesDropdown(targetEmployeeId);
            }

            var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(targetEmployeeId);
            ViewBag.Employee = employee;

            var history = await _historyHandler.HandleAsync(targetEmployeeId);
            return View(history);
        }

        private async Task LoadAvailableSessionsDropdown(int? selectedSessionId = null)
        {
            var sessions = _unitOfWork.Repository<TrainingSession>().Query()
                .Where(s => s.Status == SessionStatus.Scheduled && s.EndDate >= DateTime.Today)
                .Select(s => new
                {
                    s.Id,
                    s.Capacity,
                    EnrolledCount = s.Enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                    CourseTitle = s.Course.Title,
                    s.StartDate,
                    s.EndDate,
                    s.Location
                })
                .ToList()
                .Where(s => s.EnrolledCount < s.Capacity)
                .Select(s => new
                {
                    s.Id,
                    DisplayText = $"{s.CourseTitle} | {s.StartDate:yyyy-MM-dd} to {s.EndDate:yyyy-MM-dd} ({s.Capacity - s.EnrolledCount} seats left) - {s.Location}"
                })
                .ToList();

            ViewBag.Sessions = new SelectList(sessions, "Id", "DisplayText", selectedSessionId);
        }

        private async Task LoadEmployeesDropdown(int? selectedEmployeeId = null)
        {
            var employees = (await _unitOfWork.Repository<Employee>().GetAllAsync())
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .Select(e => new
                {
                    e.Id,
                    DisplayText = $"{e.FullName} ({e.EmployeeNumber})"
                })
                .ToList();

            ViewBag.Employees = new SelectList(employees, "Id", "DisplayText", selectedEmployeeId);
        }

        private async Task LoadFilterDropdowns(int? selectedSessionId, int? selectedEmployeeId)
        {
            var employees = (await _unitOfWork.Repository<Employee>().GetAllAsync())
                .OrderBy(e => e.FullName)
                .Select(e => new { e.Id, e.FullName })
                .ToList();
            ViewBag.FilterEmployees = new SelectList(employees, "Id", "FullName", selectedEmployeeId);

            var sessions = _unitOfWork.Repository<TrainingSession>().Query()
                .OrderByDescending(s => s.StartDate)
                .Select(s => new
                {
                    s.Id,
                    DisplayText = $"{s.Course.Title} ({s.StartDate:yyyy-MM-dd})"
                })
                .ToList();
            ViewBag.FilterSessions = new SelectList(sessions, "Id", "DisplayText", selectedSessionId);
        }
    }
}
