using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Infrastructure.Data;

namespace CorporateTrainingSystem.Infrastructure.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly AppDbContext _context;

        public AuditLogger(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string action, string? actorUserId, string? actorEmail, string? details = null)
        {
            _context.AuditLogEntries.Add(new AuditLogEntry
            {
                Action = action,
                ActorUserId = actorUserId,
                ActorEmail = actorEmail,
                Details = details,
                TimestampUtc = DateTime.UtcNow
            });

            // Saved independently of the main unit of work so an audit entry
            // is recorded even if it's logged right before an operation.
            await _context.SaveChangesAsync();
        }
    }
}