namespace CorporateTrainingSystem.Domain.Entities
{
    public class Attendance
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public bool IsPresent { get; set; }
        public string? Notes { get; set; }
    }
}