using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Assessments.RecordAssessmentResult
{
    public class RecordAssessmentResultValidator : AbstractValidator<RecordAssessmentResultCommand>
    {
        public RecordAssessmentResultValidator()
        {
            RuleFor(x => x.EnrollmentId).GreaterThan(0);

            // BR-03: Score must be within 0-100
            RuleFor(x => x.Score)
                .InclusiveBetween(0, 100)
                .WithMessage("Score must be between 0 and 100 .");
        }
    }
}