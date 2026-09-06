using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;

namespace CorporateTrainingSystem.Application.Features.Assessments.RecordAssessmentResult
{
    public class RecordAssessmentResultHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecordAssessmentResultHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RecordAssessmentResultResult> HandleAsync(RecordAssessmentResultCommand command)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(command.EnrollmentId);
            if (enrollment == null)
            {
                return new RecordAssessmentResultResult { Success = false, ErrorMessage = "Enrollment not found." };
            }

            // Need the course's passing score to determine pass/fail
            var session = await _unitOfWork.Repository<TrainingSession>().GetByIdAsync(enrollment.TrainingSessionId);
            if (session == null)
            {
                return new RecordAssessmentResultResult { Success = false, ErrorMessage = "Training session not found." };
            }

            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(session.CourseId);
            if (course == null)
            {
                return new RecordAssessmentResultResult { Success = false, ErrorMessage = "Course not found." };
            }

            bool passed = command.Score >= course.PassingScore;

            var existing = _unitOfWork.Repository<AssessmentResult>().Query()
                .FirstOrDefault(a => a.EnrollmentId == command.EnrollmentId);

            if (existing != null)
            {
                existing.Score = command.Score;
                existing.Passed = passed;
                existing.AssessedAt = DateTime.UtcNow;
                _unitOfWork.Repository<AssessmentResult>().Update(existing);
            }
            else
            {
                var result = new AssessmentResult
                {
                    EnrollmentId = command.EnrollmentId,
                    Score = command.Score,
                    Passed = passed,
                    AssessedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<AssessmentResult>().AddAsync(result);
            }

            // Mark enrollment as Completed once assessed
            enrollment.Status = EnrollmentStatus.Completed;
            _unitOfWork.Repository<Enrollment>().Update(enrollment);

            await _unitOfWork.SaveChangesAsync();

            return new RecordAssessmentResultResult { Success = true, Passed = passed };
        }
    }
}