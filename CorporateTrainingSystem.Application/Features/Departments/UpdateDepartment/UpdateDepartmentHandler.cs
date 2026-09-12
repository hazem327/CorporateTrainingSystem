using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Departments.UpdateDepartment
{
    public class UpdateDepartmentHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(UpdateDepartmentCommand command)
        {
            var department = await _unitOfWork.Repository<Department>().GetByIdAsync(command.Id);
            if (department == null) return false;

            department.Name = command.Name;
            department.Description = command.Description;

            _unitOfWork.Repository<Department>().Update(department);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}