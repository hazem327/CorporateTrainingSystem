using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Enrollments.CancelEnrollment
{
    public class CancelEnrollmentHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelEnrollmentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CancelEnrollmentResult> HandleAsync(CancelEnrollmentCommand command)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(command.EnrollmentId);
            if (enrollment == null)
            {
                return new CancelEnrollmentResult
                {
                    Success = false,
                    ErrorMessage = "Enrollment not found."
                };
            }

            if (enrollment.Status == EnrollmentStatus.Cancelled)
            {
                return new CancelEnrollmentResult
                {
                    Success = false,
                    ErrorMessage = "Enrollment is already cancelled."
                };
            }

            if (enrollment.Status == EnrollmentStatus.Completed)
            {
                return new CancelEnrollmentResult
                {
                    Success = false,
                    ErrorMessage = "Cannot cancel an enrollment that is already completed."
                };
            }

            // BR-07: Soft-delete / status change rather than physical deletion
            enrollment.Status = EnrollmentStatus.Cancelled;
            _unitOfWork.Repository<Enrollment>().Update(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return new CancelEnrollmentResult
            {
                Success = true
            };
        }
    }
}
