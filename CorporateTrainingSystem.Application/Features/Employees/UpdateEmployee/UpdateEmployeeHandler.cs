using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmployeeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(UpdateEmployeeCommand command)
        {
            var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(command.Id);
            if (employee == null) return false;

            employee.EmployeeNumber = command.EmployeeNumber;
            employee.FullName = command.FullName;
            employee.Email = command.Email;
            employee.JobTitle = command.JobTitle;
            employee.HireDate = command.HireDate;
            employee.DepartmentId = command.DepartmentId;

            _unitOfWork.Repository<Employee>().Update(employee);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}