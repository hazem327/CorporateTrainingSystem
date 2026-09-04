using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Courses.CreateCourse
{
    public class CreateCourseHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCourseResult> HandleAsync(CreateCourseCommand command)
        {
            var course = new Course
            {
                Title = command.Title,
                Description = command.Description,
                Category = command.Category,
                DurationHours = command.DurationHours,
                PassingScore = command.PassingScore,
                CertificateValidityMonths = command.CertificateValidityMonths,
                IsActive = true
            };

            await _unitOfWork.Repository<Course>().AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return new CreateCourseResult { Id = course.Id, Title = course.Title };
        }
    }
}