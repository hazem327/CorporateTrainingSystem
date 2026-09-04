using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Employees.ListEmployees
{
    public class ListEmployeesHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListEmployeesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<EmployeeListItem>> HandleAsync()
        {
            var employees = _unitOfWork.Repository<Employee>().Query()
                .Select(e => new EmployeeListItem
                {
                    Id = e.Id,
                    EmployeeNumber = e.EmployeeNumber,
                    FullName = e.FullName,
                    Email = e.Email,
                    JobTitle = e.JobTitle,
                    DepartmentName = e.Department.Name,
                    IsActive = e.IsActive
                })
                .ToList();

            return Task.FromResult(employees);
        }
    }
}