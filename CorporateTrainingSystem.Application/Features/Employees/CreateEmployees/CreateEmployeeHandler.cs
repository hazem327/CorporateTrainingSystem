using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Employees.CreateEmployee
{
    public class CreateEmployeeHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEmployeeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateEmployeeResult> HandleAsync(CreateEmployeeCommand command)
        {
            var employee = new Employee
            {
                EmployeeNumber = command.EmployeeNumber,
                FullName = command.FullName,
                Email = command.Email,
                JobTitle = command.JobTitle,
                HireDate = command.HireDate,
                DepartmentId = command.DepartmentId,
                IsActive = true
            };

            await _unitOfWork.Repository<Employee>().AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return new CreateEmployeeResult { Id = employee.Id, FullName = employee.FullName };
        }
    }
}