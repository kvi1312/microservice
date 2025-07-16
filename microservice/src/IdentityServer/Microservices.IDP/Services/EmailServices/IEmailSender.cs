namespace IdentityServer.Services.EmailServices;

public interface IEmailSender
{
    void SendEmail(string recipient, string subject, string body, bool isBodyHtml = false, string sender = null);
}