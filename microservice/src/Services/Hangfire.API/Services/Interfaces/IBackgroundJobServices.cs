namespace Hangfire.API.Services.Interfaces;

public interface IBackgroundJobServices
{
    string? SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
}
