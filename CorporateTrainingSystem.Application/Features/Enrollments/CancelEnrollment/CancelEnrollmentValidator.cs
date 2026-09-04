using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Enrollments.CancelEnrollment
{
    public class CancelEnrollmentValidator : AbstractValidator<CancelEnrollmentCommand>
    {
        public CancelEnrollmentValidator()
        {
            RuleFor(x => x.EnrollmentId)
                .GreaterThan(0)
                .WithMessage("Invalid enrollment ID.");
        }
    }
}
