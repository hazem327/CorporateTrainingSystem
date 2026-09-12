using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CorporateTrainingSystem.Application.Features.Courses.ListCourses
{
    public class ListCoursesHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "ActiveCoursesList";

        public ListCoursesHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public Task<List<CourseListItem>> HandleAsync()
        {
            if (_cache.TryGetValue(CacheKey, out List<CourseListItem>? cached) && cached != null)
            {
                return Task.FromResult(cached);
            }

            var courses = _unitOfWork.Repository<Course>().Query()
                .Where(c => c.IsActive)
                .Select(c => new CourseListItem
                {
                    Id = c.Id,
                    Title = c.Title,
                    Category = c.Category,
                    DurationHours = c.DurationHours,
                    IsActive = c.IsActive
                })
                .ToList();

            // Cache for 5 minutes - course list is read-heavy and changes infrequently.
            _cache.Set(CacheKey, courses, TimeSpan.FromMinutes(5));

            return Task.FromResult(courses);
        }
    }
}