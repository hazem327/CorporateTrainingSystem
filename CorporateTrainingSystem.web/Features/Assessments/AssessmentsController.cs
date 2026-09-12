using CorporateTrainingSystem.Application.Features.Assessments.RecordAssessmentResult;
using CorporateTrainingSystem.Application.Features.Assessments.RecordAttendance;
using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CorporateTrainingSystem.Web.Features.Assessments
{
    [Authorize(Roles = "Administrator,TrainingManager,Instructor")]
    public class AssessmentsController : Controller
    {
        private readonly RecordAttendanceHandler _attendanceHandler;
        private readonly RecordAssessmentResultHandler _resultHandler;
        private readonly IValidator<RecordAssessmentResultCommand> _resultValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentsController(
            RecordAttendanceHandler attendanceHandler,
            RecordAssessmentResultHandler resultHandler,
            IValidator<RecordAssessmentResultCommand> resultValidator,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _attendanceHandler = attendanceHandler;
            _resultHandler = resultHandler;
            _resultValidator = resultValidator;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Record(int enrollmentId)
        {
            if (!await CanManageThisEnrollment(enrollmentId))
            {
                return Forbid();
            }

            var command = new RecordAssessmentResultCommand { EnrollmentId = enrollmentId };
            return View(command);
        }

        private async Task<bool> CanManageThisEnrollment(int enrollmentId)
        {
            // Admins and Training Managers can manage any session's assessments.
            if (User.IsInRole("Administrator") || User.IsInRole("TrainingManager"))
            {
                return true;
            }

            // Instructors may only manage sessions they are assigned to teach.
            if (User.IsInRole("Instructor"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.EmployeeId == null) return false;

                var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(enrollmentId);
                if (enrollment == null) return false;

                var session = await _unitOfWork.Repository<TrainingSession>().GetByIdAsync(enrollment.TrainingSessionId);
                return session != null && session.InstructorId == currentUser.EmployeeId.Value;
            }

            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Record(RecordAssessmentResultCommand command, bool isPresent)
        {
            if (!await CanManageThisEnrollment(command.EnrollmentId))
            {
                return Forbid();
            }

            var validation = await _resultValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                return View(command);
            }

            await _attendanceHandler.HandleAsync(new RecordAttendanceCommand
            {
                EnrollmentId = command.EnrollmentId,
                IsPresent = isPresent
            });

            var result = await _resultHandler.HandleAsync(command);

            TempData["Success"] = result.Passed
                ? "Assessment recorded — employee passed."
                : "Assessment recorded — employee did not pass.";

            return RedirectToAction("Index", "Enrollments");
        }
    }
}