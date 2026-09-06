namespace CorporateTrainingSystem.Application.Features.Certifications.ListCertifications
{
    public class CertificationListItem
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}