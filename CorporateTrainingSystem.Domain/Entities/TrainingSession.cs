namespace CorporateTrainingSystem.Domain.Entities
{
    public enum SessionStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }

    public class TrainingSession
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int InstructorId { get; set; }        
        public Employee Instructor { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; } = string.Empty;
        public SessionStatus Status { get; set; } = SessionStatus.Scheduled;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}