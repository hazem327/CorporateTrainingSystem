namespace CorporateTrainingSystem.Application.Features.Courses.CreateCourse
{
    public class CreateCourseCommand
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        public int PassingScore { get; set; }
        public int CertificateValidityMonths { get; set; }
    }
}