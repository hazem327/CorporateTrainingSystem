namespace CorporateTrainingSystem.Domain.Entities
{
    public enum CertificationStatus
    {
        Valid,
        ExpiringSoon,
        Expired
    }

    public class Certification
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public string CertificateNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public CertificationStatus Status { get; set; } = CertificationStatus.Valid;
    }
}