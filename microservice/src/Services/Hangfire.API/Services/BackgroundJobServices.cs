using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services.Interfaces;
using Shared.Services.Email;
using ILogger = Serilog.ILogger;
namespace Hangfire.API.Services;

public class BackgroundJobServices : IBackgroundJobServices
{
    private readonly IScheduledServices _jobService;
    private readonly ISmtpEmailService _smtpEmailService;
    private readonly ILogger _logger;

    public BackgroundJobServices(ILogger logger, ISmtpEmailService smtpEmailService, IScheduledServices jobService)
    {
        _logger = logger;
        _smtpEmailService = smtpEmailService;
        _jobService = jobService;
    }

    public string SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt)
    {
        var emailRequest = new MailRequest
        {
            ToAddress = email,
            Body = emailContent,
            Subject = subject,
        };

        try
        {
            var jobId = _jobService.Schedule(() => _smtpEmailService.SendEmail(emailRequest), enqueueAt);
            _logger.Information($"Send email to {email} with subject: {subject}");
            return jobId;
        }
        catch (Exception ex) {
            _logger.Error($"Failed due to an error with the email services : {ex.Message}");
        }

        return string.Empty;
    }
}
