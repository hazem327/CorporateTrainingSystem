namespace CorporateTrainingSystem.Application.Features.Certifications.IssueCertificate
{
    public class IssueCertificateCommand
    {
        public int EnrollmentId { get; set; }
        public string? ActorUserId { get; set; }
        public string? ActorEmail { get; set; }
    }
}