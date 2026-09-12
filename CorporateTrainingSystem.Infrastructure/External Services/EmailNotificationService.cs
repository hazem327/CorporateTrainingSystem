using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace CorporateTrainingSystem.Infrastructure.ExternalServices
{
    // Simulates an external email provider call. In place of a real SMTP/API
    // integration, this demonstrates the resilience pattern (timeout, retry,
    // circuit breaker) that would wrap any real external dependency.
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;
        private static readonly AsyncCircuitBreakerPolicy _circuitBreaker =
            Policy.Handle<Exception>()
                  .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(30));

        public EmailNotificationService(ILogger<EmailNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendCertificateIssuedNotificationAsync(string employeeEmail, string certificateNumber)
        {
            var retryPolicy = Policy
                .Handle<TimeoutException>()
                .WaitAndRetryAsync(2, attempt => TimeSpan.FromMilliseconds(200 * attempt));

            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(3));

            var combinedPolicy = Policy.WrapAsync(retryPolicy, _circuitBreaker, timeoutPolicy);

            try
            {
                await combinedPolicy.ExecuteAsync(async () =>
                {
                    await SimulateExternalCallAsync(employeeEmail, certificateNumber);
                });

                return true;
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Email service circuit breaker is open; notification skipped for {Email}.", employeeEmail);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send certificate notification to {Email} after retries.", employeeEmail);
                return false;
            }
        }

        private static Task SimulateExternalCallAsync(string email, string certNumber)
        {
            // Placeholder for a real external call (e.g. SendGrid, SES).
            // Simulates network latency; a real integration would call an HTTP client here.
            return Task.Delay(50);
        }
    }
}