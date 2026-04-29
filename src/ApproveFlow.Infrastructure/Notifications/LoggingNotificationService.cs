using ApproveFlow.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace ApproveFlow.Infrastructure.Notifications;

public sealed class LoggingNotificationService : INotificationService
{
    private readonly ILogger<LoggingNotificationService> _logger;

    public LoggingNotificationService(ILogger<LoggingNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification To={To} Subject={Subject} Body={Body}", to, subject, body);
        return Task.CompletedTask;
    }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
