using CorporateTrainingSystem.Application.Features.Certifications.IssueCertificate;
using CorporateTrainingSystem.Application.Features.Certifications.ListCertifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateTrainingSystem.Web.Features.Certifications
{
    [Authorize]
    public class CertificationsController : Controller
    {
        private readonly IssueCertificateHandler _issueHandler;
        private readonly ListCertificationsHandler _listHandler;

        public CertificationsController(
            IssueCertificateHandler issueHandler,
            ListCertificationsHandler listHandler)
        {
            _issueHandler = issueHandler;
            _listHandler = listHandler;
        }

        public async Task<IActionResult> Index()
        {
            var certifications = await _listHandler.HandleAsync();
            return View(certifications);
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue(int enrollmentId)
        {
            var result = await _issueHandler.HandleAsync(new IssueCertificateCommand { EnrollmentId = enrollmentId });

            TempData[result.Success ? "Success" : "Error"] = result.Success
                ? $"Certificate {result.CertificateNumber} issued."
                : result.ErrorMessage;

            return RedirectToAction("Index", "Enrollments");
        }
    }
}