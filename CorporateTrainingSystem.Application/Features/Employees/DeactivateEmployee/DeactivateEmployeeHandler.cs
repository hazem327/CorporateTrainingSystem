using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Employees.DeactivateEmployee
{
    public class DeactivateEmployeeHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateEmployeeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(int employeeId)
        {
            var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(employeeId);
            if (employee == null) return false;

            // Deactivation, not deletion - preserves history for existing
            // enrollments, assessments, and certifications tied to this employee.
            employee.IsActive = false;

            _unitOfWork.Repository<Employee>().Update(employee);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}