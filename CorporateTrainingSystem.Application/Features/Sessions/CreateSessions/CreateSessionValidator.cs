using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Sessions.CreateSession
{
    public class CreateSessionValidator : AbstractValidator<CreateSessionCommand>
    {
        public CreateSessionValidator()
        {
            RuleFor(x => x.CourseId).GreaterThan(0);
            RuleFor(x => x.InstructorId).GreaterThan(0);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.Location).NotEmpty().MaximumLength(200);
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be on or after the start date.");
        }
    }
}