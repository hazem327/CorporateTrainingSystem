using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Certifications.IssueCertificate
{
    public class IssueCertificateHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public IssueCertificateHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IssueCertificateResult> HandleAsync(IssueCertificateCommand command)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(command.EnrollmentId);
            if (enrollment == null)
            {
                return new IssueCertificateResult { Success = false, ErrorMessage = "Enrollment not found." };
            }

            // BR-04: Certificate can only be issued if the employee passed the assessment.
            var assessment = _unitOfWork.Repository<AssessmentResult>().Query()
                .FirstOrDefault(a => a.EnrollmentId == command.EnrollmentId);

            if (assessment == null)
            {
                return new IssueCertificateResult { Success = false, ErrorMessage = "No assessment result found for this enrollment (BR-04)." };
            }

            if (!assessment.Passed)
            {
                return new IssueCertificateResult { Success = false, ErrorMessage = "Cannot issue a certificate: employee did not pass the assessment (BR-04)." };
            }

            // Prevent duplicate certificates for the same enrollment
            var existingCert = _unitOfWork.Repository<Certification>().Query()
                .FirstOrDefault(c => c.EnrollmentId == command.EnrollmentId);

            if (existingCert != null)
            {
                return new IssueCertificateResult { Success = false, ErrorMessage = "A certificate has already been issued for this enrollment." };
            }

            var session = await _unitOfWork.Repository<TrainingSession>().GetByIdAsync(enrollment.TrainingSessionId);
            if (session == null)
            {
                return new IssueCertificateResult { Success = false, ErrorMessage = "Training session not found." };
            }

            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(session.CourseId);
            if (course == null)
            {
                return new IssueCertificateResult { Success = false, ErrorMessage = "Course not found." };
            }

            var issueDate = DateTime.UtcNow;

            // BR-06: Expiry is calculated from the course's certificate validity period.
            var expiryDate = issueDate.AddMonths(course.CertificateValidityMonths);

            var certificateNumber = $"CERT-{issueDate:yyyyMMdd}-{enrollment.Id:D5}";

            var certification = new Certification
            {
                EmployeeId = enrollment.EmployeeId,
                CourseId = course.Id,
                EnrollmentId = enrollment.Id,
                CertificateNumber = certificateNumber,
                IssueDate = issueDate,
                ExpiryDate = expiryDate,
                Status = CertificationStatus.Valid
            };

            await _unitOfWork.Repository<Certification>().AddAsync(certification);
            await _unitOfWork.SaveChangesAsync();

            return new IssueCertificateResult { Success = true, CertificateNumber = certificateNumber };
        }
    }
}