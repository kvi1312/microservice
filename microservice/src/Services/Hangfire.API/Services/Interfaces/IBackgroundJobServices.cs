using Contracts.ScheduledJobs;

namespace Hangfire.API.Services.Interfaces;

public interface IBackgroundJobServices
{
    IScheduledServices ScheduledServices { get; }
    string? SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
}
