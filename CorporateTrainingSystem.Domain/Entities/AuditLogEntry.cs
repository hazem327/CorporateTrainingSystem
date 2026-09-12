namespace CorporateTrainingSystem.Domain.Entities
{
    public class AuditLogEntry
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;       //  "EnrollmentCancelled", "CertificateIssued"
        public string? ActorUserId { get; set; }                  // ApplicationUser.Id of who did it
        public string? ActorEmail { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }                      // free-text context, e.g. "EnrollmentId=42"
    }
}