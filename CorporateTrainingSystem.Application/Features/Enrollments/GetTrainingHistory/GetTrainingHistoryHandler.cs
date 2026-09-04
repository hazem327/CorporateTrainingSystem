using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Enrollments.GetTrainingHistory
{
    public class GetTrainingHistoryHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTrainingHistoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<EmployeeTrainingHistoryItem>> HandleAsync(int employeeId)
        {
            var history = _unitOfWork.Repository<Enrollment>().Query()
                .Where(e => e.EmployeeId == employeeId)
                .OrderByDescending(e => e.TrainingSession.StartDate)
                .Select(e => new EmployeeTrainingHistoryItem
                {
                    EnrollmentId = e.Id,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.Employee.FullName,
                    EmployeeNumber = e.Employee.EmployeeNumber,
                    DepartmentName = e.Employee.Department.Name,
                    CourseId = e.TrainingSession.CourseId,
                    CourseTitle = e.TrainingSession.Course.Title,
                    Category = e.TrainingSession.Course.Category,
                    DurationHours = e.TrainingSession.Course.DurationHours,
                    PassingScore = e.TrainingSession.Course.PassingScore,
                    SessionId = e.TrainingSessionId,
                    SessionStartDate = e.TrainingSession.StartDate,
                    SessionEndDate = e.TrainingSession.EndDate,
                    Location = e.TrainingSession.Location,
                    InstructorName = e.TrainingSession.Instructor.FullName,
                    EnrolledAt = e.EnrolledAt,
                    Status = e.Status.ToString(),
                    IsPresent = e.Attendance != null ? e.Attendance.IsPresent : (bool?)null,
                    AssessmentScore = e.AssessmentResult != null ? e.AssessmentResult.Score : (int?)null,
                    Passed = e.AssessmentResult != null ? e.AssessmentResult.Passed : (bool?)null,
                    CertificateNumber = e.Certification != null ? e.Certification.CertificateNumber : null,
                    CertificateIssueDate = e.Certification != null ? e.Certification.IssueDate : (DateTime?)null,
                    CertificateExpiryDate = e.Certification != null ? e.Certification.ExpiryDate : (DateTime?)null,
                    CertificateStatus = e.Certification != null ? e.Certification.Status.ToString() : null
                })
                .ToList();

            return Task.FromResult(history);
        }
    }
}
