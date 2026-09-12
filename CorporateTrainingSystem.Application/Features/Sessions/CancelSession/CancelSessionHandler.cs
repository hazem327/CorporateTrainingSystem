using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Sessions.CancelSession
{
    public class CancelSessionHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelSessionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CancelSessionResult> HandleAsync(int sessionId)
        {
            var session = await _unitOfWork.Repository<TrainingSession>().GetByIdAsync(sessionId);
            if (session == null)
            {
                return new CancelSessionResult { Success = false, ErrorMessage = "Training session not found." };
            }

            if (session.Status == SessionStatus.Cancelled)
            {
                return new CancelSessionResult { Success = false, ErrorMessage = "Session is already cancelled." };
            }

            if (session.Status == SessionStatus.Completed)
            {
                return new CancelSessionResult { Success = false, ErrorMessage = "Cannot cancel a session that has already been completed." };
            }

            // Only future/scheduled sessions can be cancelled - preserves
            // historical record for sessions already in progress or completed.
            if (session.StartDate < DateTime.Today)
            {
                return new CancelSessionResult { Success = false, ErrorMessage = "Cannot cancel a session that has already started." };
            }

            // Status change, not deletion - preserves the session record and
            // any existing enrollments for historical/audit purposes.
            session.Status = SessionStatus.Cancelled;
            _unitOfWork.Repository<TrainingSession>().Update(session);
            await _unitOfWork.SaveChangesAsync();

            return new CancelSessionResult { Success = true };
        }
    }
}