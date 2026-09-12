namespace CorporateTrainingSystem.Application.Features.Enrollments.CancelEnrollment
{
    public class CancelEnrollmentCommand
    {
        public int EnrollmentId { get; set; }
        public string? ActorUserId { get; set; }
        public string? ActorEmail { get; set; }
    }
}