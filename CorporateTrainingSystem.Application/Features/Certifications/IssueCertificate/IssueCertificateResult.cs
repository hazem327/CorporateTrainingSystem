namespace CorporateTrainingSystem.Application.Features.Certifications.IssueCertificate
{
    public class IssueCertificateResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? CertificateNumber { get; set; }
    }
}