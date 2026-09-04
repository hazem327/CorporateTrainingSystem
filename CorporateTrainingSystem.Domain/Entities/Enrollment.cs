namespace CorporateTrainingSystem.Domain.Entities
{
    public enum EnrollmentStatus
    {
        Active,
        Cancelled,
        Completed
    }

    public class Enrollment
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int TrainingSessionId { get; set; }
        public TrainingSession TrainingSession { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        public Attendance? Attendance { get; set; }
        public AssessmentResult? AssessmentResult { get; set; }
        public Certification? Certification { get; set; }
    }
}