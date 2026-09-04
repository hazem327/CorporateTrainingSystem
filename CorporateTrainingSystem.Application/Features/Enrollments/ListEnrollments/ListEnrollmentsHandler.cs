using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Enrollments.ListEnrollments
{
    public class ListEnrollmentsHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListEnrollmentsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<EnrollmentListItem>> HandleAsync(ListEnrollmentsQuery? query = null)
        {
            var queryable = _unitOfWork.Repository<Enrollment>().Query();

            if (query != null)
            {
                if (query.EmployeeId.HasValue)
                {
                    queryable = queryable.Where(e => e.EmployeeId == query.EmployeeId.Value);
                }

                if (query.SessionId.HasValue)
                {
                    queryable = queryable.Where(e => e.TrainingSessionId == query.SessionId.Value);
                }

                if (query.Status.HasValue)
                {
                    queryable = queryable.Where(e => e.Status == query.Status.Value);
                }
            }

            var enrollments = queryable
                .OrderByDescending(e => e.EnrolledAt)
                .Select(e => new EnrollmentListItem
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.Employee.FullName,
                    EmployeeNumber = e.Employee.EmployeeNumber,
                    DepartmentName = e.Employee.Department.Name,
                    TrainingSessionId = e.TrainingSessionId,
                    CourseTitle = e.TrainingSession.Course.Title,
                    InstructorName = e.TrainingSession.Instructor.FullName,
                    StartDate = e.TrainingSession.StartDate,
                    EndDate = e.TrainingSession.EndDate,
                    Location = e.TrainingSession.Location,
                    EnrolledAt = e.EnrolledAt,
                    Status = e.Status.ToString(),
                    CanCancel = e.Status == EnrollmentStatus.Active && e.TrainingSession.Status != SessionStatus.Completed
                })
                .ToList();

            return Task.FromResult(enrollments);
        }
    }
}
