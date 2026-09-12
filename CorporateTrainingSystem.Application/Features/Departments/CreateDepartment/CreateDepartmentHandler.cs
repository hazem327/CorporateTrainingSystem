using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Departments.CreateDepartment
{
    public class CreateDepartmentHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(CreateDepartmentCommand command)
        {
            var department = new Department
            {
                Name = command.Name,
                Description = command.Description
            };

            await _unitOfWork.Repository<Department>().AddAsync(department);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}