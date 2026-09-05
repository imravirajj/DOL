using DOL.Identity.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace DOL.Identity.Infrastructure.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("==================================================");
        _logger.LogInformation("[EMAIL SENT] To: {To} | Subject: {Subject}", to, subject);
        _logger.LogInformation("Body: {Body}", body);
        _logger.LogInformation("==================================================");

        return Task.CompletedTask;
    }
}
