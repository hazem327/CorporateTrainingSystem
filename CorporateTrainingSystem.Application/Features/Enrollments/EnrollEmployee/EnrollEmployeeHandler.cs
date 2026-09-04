using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Enrollments.EnrollEmployee
{
    public class EnrollEmployeeHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollEmployeeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EnrollEmployeeResult> HandleAsync(EnrollEmployeeCommand command)
        {
            var session = await _unitOfWork.Repository<TrainingSession>().GetByIdAsync(command.TrainingSessionId);
            if (session == null)
            {
                return new EnrollEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Training session not found."
                };
            }

            // BR-05: A cancelled session cannot accept new enrollments.
            if (session.Status == SessionStatus.Cancelled)
            {
                return new EnrollEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Cannot enroll in a cancelled training session (BR-05)."
                };
            }

            if (session.Status == SessionStatus.Completed)
            {
                return new EnrollEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Cannot enroll in a completed training session."
                };
            }

            // Validate employee
            var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(command.EmployeeId);
            if (employee == null)
            {
                return new EnrollEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Employee not found."
                };
            }

            if (!employee.IsActive)
            {
                return new EnrollEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Cannot enroll an inactive employee."
                };
            }

            // BR-01: An employee cannot be enrolled more than once in the same session.
            var existingActiveEnrollment = _unitOfWork.Repository<Enrollment>().Query()
                .FirstOrDefault(e => e.EmployeeId == command.EmployeeId &&
                                     e.TrainingSessionId == command.TrainingSessionId &&
                                     e.Status == EnrollmentStatus.Active);

            if (existingActiveEnrollment != null)
            {
                return new EnrollEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Employee is already actively enrolled in this training session (BR-01)."
                };
            }

            // BR-02: Enrollment cannot exceed the session capacity.
            var activeCount = _unitOfWork.Repository<Enrollment>().Query()
                .Count(e => e.TrainingSessionId == command.TrainingSessionId &&
                            e.Status == EnrollmentStatus.Active);

            if (activeCount >= session.Capacity)
            {
                return new EnrollEmployeeResult
                {
                    Success = false,
                    ErrorMessage = $"Cannot enroll: Session has reached maximum capacity of {session.Capacity} (BR-02)."
                };
            }

            var enrollment = new Enrollment
            {
                EmployeeId = command.EmployeeId,
                TrainingSessionId = command.TrainingSessionId,
                EnrolledAt = DateTime.UtcNow,
                Status = EnrollmentStatus.Active
            };

            await _unitOfWork.Repository<Enrollment>().AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return new EnrollEmployeeResult
            {
                Success = true,
                EnrollmentId = enrollment.Id
            };
        }
    }
}
