using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Certifications.ListCertifications
{
    public class ListCertificationsHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListCertificationsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<CertificationListItem>> HandleAsync()
        {
            var now = DateTime.UtcNow;

            var certifications = _unitOfWork.Repository<Certification>().Query()
                .Select(c => new CertificationListItem
                {
                    Id = c.Id,
                    EmployeeName = c.Employee.FullName,
                    CourseTitle = c.Course.Title,
                    CertificateNumber = c.CertificateNumber,
                    IssueDate = c.IssueDate,
                    ExpiryDate = c.ExpiryDate,
                    // Computed dynamically rather than trusting a possibly-stale Status column
                    Status = c.ExpiryDate < now ? "Expired"
                             : c.ExpiryDate < now.AddDays(30) ? "ExpiringSoon"
                             : "Valid"
                })
                .OrderByDescending(c => c.IssueDate)
                .ToList();

            return Task.FromResult(certifications);
        }
    }
}