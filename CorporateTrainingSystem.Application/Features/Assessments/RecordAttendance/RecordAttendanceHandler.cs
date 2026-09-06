using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Assessments.RecordAttendance
{
    public class RecordAttendanceHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecordAttendanceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RecordAttendanceResult> HandleAsync(RecordAttendanceCommand command)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(command.EnrollmentId);
            if (enrollment == null)
            {
                return new RecordAttendanceResult { Success = false, ErrorMessage = "Enrollment not found." };
            }

            // One attendance record per enrollment - check if it already exists
            var existing = _unitOfWork.Repository<Attendance>().Query()
                .FirstOrDefault(a => a.EnrollmentId == command.EnrollmentId);

            if (existing != null)
            {
                existing.IsPresent = command.IsPresent;
                existing.Notes = command.Notes;
                _unitOfWork.Repository<Attendance>().Update(existing);
            }
            else
            {
                var attendance = new Attendance
                {
                    EnrollmentId = command.EnrollmentId,
                    IsPresent = command.IsPresent,
                    Notes = command.Notes
                };
                await _unitOfWork.Repository<Attendance>().AddAsync(attendance);
            }

            await _unitOfWork.SaveChangesAsync();
            return new RecordAttendanceResult { Success = true };
        }
    }
}