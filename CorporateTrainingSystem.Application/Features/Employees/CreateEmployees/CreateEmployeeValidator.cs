using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Employees.CreateEmployee
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(100);
            RuleFor(x => x.HireDate).NotEmpty();
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }
}