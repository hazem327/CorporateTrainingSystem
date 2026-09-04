namespace CorporateTrainingSystem.Domain.Entities
{
    public class AssessmentResult
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public int Score { get; set; }                 // 0-100, enforced in validator
        public bool Passed { get; set; }
        public DateTime AssessedAt { get; set; } = DateTime.UtcNow;
    }
}