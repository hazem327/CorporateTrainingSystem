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

        // employeeIdFilter: null means "no restriction" (Admin/TrainingManager/Instructor).
        // A non-null value restricts results to that employee only (used for the Employee role).
        public Task<List<CertificationListItem>> HandleAsync(int? employeeIdFilter = null)
        {
            var now = DateTime.UtcNow;

            var query = _unitOfWork.Repository<Certification>().Query();

            if (employeeIdFilter.HasValue)
            {
                query = query.Where(c => c.EmployeeId == employeeIdFilter.Value);
            }

            var certifications = query
                .Select(c => new CertificationListItem
                {
                    Id = c.Id,
                    EmployeeName = c.Employee.FullName,
                    CourseTitle = c.Course.Title,
                    CertificateNumber = c.CertificateNumber,
                    IssueDate = c.IssueDate,
                    ExpiryDate = c.ExpiryDate,
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