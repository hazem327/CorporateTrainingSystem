namespace CorporateTrainingSystem.Application.Features.Enrollments.GetTrainingHistory
{
    public class EmployeeTrainingHistoryItem
    {
        public int EnrollmentId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeNumber { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        public int PassingScore { get; set; }

        public int SessionId { get; set; }
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;

        public DateTime EnrolledAt { get; set; }
        public string Status { get; set; } = string.Empty;

        // Attendance & Assessment
        public bool? IsPresent { get; set; }
        public int? AssessmentScore { get; set; }
        public bool? Passed { get; set; }

        // Certification
        public string? CertificateNumber { get; set; }
        public DateTime? CertificateIssueDate { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }
        public string? CertificateStatus { get; set; }
    }
}
