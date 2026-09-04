namespace CorporateTrainingSystem.Application.Features.Courses.ListCourses
{
    public class CourseListItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        public bool IsActive { get; set; }
    }
}