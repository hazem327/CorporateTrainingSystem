using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Departments.UpdateDepartment
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }
}