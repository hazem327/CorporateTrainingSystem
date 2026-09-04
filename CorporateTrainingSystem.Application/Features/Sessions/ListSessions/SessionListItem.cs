namespace CorporateTrainingSystem.Application.Features.Sessions.ListSessions
{
    public class SessionListItem
    {
        public int Id { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}