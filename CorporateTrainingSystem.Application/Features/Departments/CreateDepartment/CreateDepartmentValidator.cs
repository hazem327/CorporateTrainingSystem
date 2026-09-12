using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Departments.CreateDepartment
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }
}