namespace CorporateTrainingSystem.Domain.Interfaces
{
    public interface IAuditLogger
    {
        Task LogAsync(string action, string? actorUserId, string? actorEmail, string? details = null);
    }
}