using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Courses.CreateCourse
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DurationHours).GreaterThan(0);
            RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);
            RuleFor(x => x.CertificateValidityMonths).GreaterThanOrEqualTo(0);
        }
    }
}