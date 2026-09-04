using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Sessions.ListSessions
{
    public class ListSessionsHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListSessionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<SessionListItem>> HandleAsync()
        {
            var sessions = _unitOfWork.Repository<TrainingSession>().Query()
                .Select(s => new SessionListItem
                {
                    Id = s.Id,
                    CourseTitle = s.Course.Title,
                    InstructorName = s.Instructor.FullName,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Capacity = s.Capacity,
                    EnrolledCount = s.Enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                    Location = s.Location,
                    Status = s.Status.ToString()
                })
                .ToList();

            return Task.FromResult(sessions);
        }
    }
}