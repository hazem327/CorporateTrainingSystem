namespace CorporateTrainingSystem.Application.Features.Enrollments.ListEnrollments
{
    public class EnrollmentListItem
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeNumber { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public int TrainingSessionId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;

        public DateTime EnrolledAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool CanCancel { get; set; }
    }
}
