using CorporateTrainingSystem.Application.Features.Assessments.RecordAssessmentResult;
using CorporateTrainingSystem.Application.Features.Assessments.RecordAttendance;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateTrainingSystem.Web.Features.Assessments
{
    [Authorize(Roles = "Administrator,TrainingManager,Instructor")]
    public class AssessmentsController : Controller
    {
        private readonly RecordAttendanceHandler _attendanceHandler;
        private readonly RecordAssessmentResultHandler _resultHandler;
        private readonly IValidator<RecordAssessmentResultCommand> _resultValidator;

        public AssessmentsController(
            RecordAttendanceHandler attendanceHandler,
            RecordAssessmentResultHandler resultHandler,
            IValidator<RecordAssessmentResultCommand> resultValidator)
        {
            _attendanceHandler = attendanceHandler;
            _resultHandler = resultHandler;
            _resultValidator = resultValidator;
        }

        [HttpGet]
        public IActionResult Record(int enrollmentId)
        {
            var command = new RecordAssessmentResultCommand { EnrollmentId = enrollmentId };
            return View(command);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Record(RecordAssessmentResultCommand command, bool isPresent)
        {
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