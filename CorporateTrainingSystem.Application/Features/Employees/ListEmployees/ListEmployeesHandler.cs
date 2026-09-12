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

        public Task<EmployeeListResult> HandleAsync(EmployeeListQuery query)
        {
            var employees = _unitOfWork.Repository<Employee>().Query();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim().ToLower();
                employees = employees.Where(e =>
                    e.FullName.ToLower().Contains(term) ||
                    e.EmployeeNumber.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term));
            }

            if (query.DepartmentId.HasValue)
            {
                employees = employees.Where(e => e.DepartmentId == query.DepartmentId.Value);
            }

            if (query.IsActive.HasValue)
            {
                employees = employees.Where(e => e.IsActive == query.IsActive.Value);
            }

            employees = query.SortBy switch
            {
                "EmployeeNumber" => query.SortDescending
                    ? employees.OrderByDescending(e => e.EmployeeNumber)
                    : employees.OrderBy(e => e.EmployeeNumber),
                "Department" => query.SortDescending
                    ? employees.OrderByDescending(e => e.Department.Name)
                    : employees.OrderBy(e => e.Department.Name),
                "HireDate" => query.SortDescending
                    ? employees.OrderByDescending(e => e.HireDate)
                    : employees.OrderBy(e => e.HireDate),
                _ => query.SortDescending
                    ? employees.OrderByDescending(e => e.FullName)
                    : employees.OrderBy(e => e.FullName),
            };

            var totalCount = employees.Count();

            var pageItems = employees
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
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

            return Task.FromResult(new EmployeeListResult
            {
                Items = pageItems,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }
    }
}