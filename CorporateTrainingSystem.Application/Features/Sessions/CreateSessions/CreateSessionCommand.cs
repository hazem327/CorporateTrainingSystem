namespace CorporateTrainingSystem.Application.Features.Sessions.CreateSession
{
    public class CreateSessionCommand
    {
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int Capacity { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}