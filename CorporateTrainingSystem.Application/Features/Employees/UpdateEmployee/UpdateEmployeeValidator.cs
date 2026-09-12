using FluentValidation;

namespace CorporateTrainingSystem.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(100);
            RuleFor(x => x.HireDate).NotEmpty();
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }
}