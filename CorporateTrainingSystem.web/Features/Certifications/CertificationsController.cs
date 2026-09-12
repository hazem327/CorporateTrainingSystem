using CorporateTrainingSystem.Application.Features.Certifications.IssueCertificate;
using CorporateTrainingSystem.Application.Features.Certifications.ListCertifications;
using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Infrastructure.ExternalServices;
using CorporateTrainingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CorporateTrainingSystem.Web.Features.Certifications
{
    [Authorize]
    public class CertificationsController : Controller
    {
        private readonly IssueCertificateHandler _issueHandler;
        private readonly ListCertificationsHandler _listHandler;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailNotificationService _emailService;

        public CertificationsController(
            IssueCertificateHandler issueHandler,
            ListCertificationsHandler listHandler,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IEmailNotificationService emailService)
        {
            _issueHandler = issueHandler;
            _listHandler = listHandler;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            int? employeeFilter = null;

            if (User.IsInRole("Employee") && !User.IsInRole("Administrator") && !User.IsInRole("TrainingManager"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                employeeFilter = currentUser?.EmployeeId;
            }

            var certifications = await _listHandler.HandleAsync(employeeFilter);
            return View(certifications);
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue(int enrollmentId)
        {
            var result = await _issueHandler.HandleAsync(new IssueCertificateCommand
            {
                EnrollmentId = enrollmentId,
                ActorUserId = _userManager.GetUserId(User),
                ActorEmail = User.Identity?.Name
            });

            if (result.Success)
            {
                TempData["Success"] = $"Certificate {result.CertificateNumber} issued.";

                // Best-effort notification - failure here should never block the
                // certificate issuance that already succeeded above.
                var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(enrollmentId);
                if (enrollment != null)
                {
                    var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(enrollment.EmployeeId);
                    if (employee != null)
                    {
                        await _emailService.SendCertificateIssuedNotificationAsync(employee.Email, result.CertificateNumber!);
                    }
                }
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("Index", "Enrollments");
        }
    }
}