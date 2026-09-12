namespace CorporateTrainingSystem.Infrastructure.ExternalServices
{
    public interface IEmailNotificationService
    {
        Task<bool> SendCertificateIssuedNotificationAsync(string employeeEmail, string certificateNumber);
    }
}