using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Courses.ListCourses
{
    public class ListCoursesHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListCoursesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<CourseListItem>> HandleAsync()
        {
            var courses = _unitOfWork.Repository<Course>().Query()
                .Select(c => new CourseListItem
                {
                    Id = c.Id,
                    Title = c.Title,
                    Category = c.Category,
                    DurationHours = c.DurationHours,
                    IsActive = c.IsActive
                })
                .ToList();

            return Task.FromResult(courses);
        }
    }
}