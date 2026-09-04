using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Sessions.CreateSession
{
    public class CreateSessionHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSessionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateSessionResult> HandleAsync(CreateSessionCommand command)
        {
            var session = new TrainingSession
            {
                CourseId = command.CourseId,
                InstructorId = command.InstructorId,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                Capacity = command.Capacity,
                Location = command.Location,
                Status = SessionStatus.Scheduled
            };

            await _unitOfWork.Repository<TrainingSession>().AddAsync(session);
            await _unitOfWork.SaveChangesAsync();

            return new CreateSessionResult { Id = session.Id };
        }
    }
}