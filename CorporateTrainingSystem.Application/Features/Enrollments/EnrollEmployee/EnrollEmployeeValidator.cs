using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Enrollments.EnrollEmployee
{
    public class EnrollEmployeeValidator : AbstractValidator<EnrollEmployeeCommand>
    {
        public EnrollEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("Please select an employee.");

            RuleFor(x => x.TrainingSessionId)
                .GreaterThan(0)
                .WithMessage("Please select a training session.");
        }
    }
}
