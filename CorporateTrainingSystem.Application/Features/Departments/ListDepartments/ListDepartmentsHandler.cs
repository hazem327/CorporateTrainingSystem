using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Departments.ListDepartments
{
    public class ListDepartmentsHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListDepartmentsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<DepartmentListItem>> HandleAsync()
        {
            var departments = _unitOfWork.Repository<Department>().Query()
                .Select(d => new DepartmentListItem
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EmployeeCount = d.Employees.Count(e => e.IsActive)
                })
                .ToList();

            return Task.FromResult(departments);
        }
    }
}