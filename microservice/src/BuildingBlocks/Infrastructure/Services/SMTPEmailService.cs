using Contracts.Services;
using Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using Shared.Services.Email;

namespace Infrastructure.Services;

public class SMTPEmailService : ISmtpEmailService
{
    private readonly ILogger _logger;
    private readonly SMTPEmailSettings _setting;
    private readonly SmtpClient _smtpClient;

    public SMTPEmailService(ILogger logger, SMTPEmailSettings setting)
    {
        _logger = logger;
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _smtpClient = new SmtpClient();
    }

    public void SendEmail(MailRequest request)
    {
        var message = SetupEmailMessage(request);
        try
        {
            _smtpClient.Connect(_setting.SmtpServer, _setting.Port, _setting.UseSsl);
            _smtpClient.Authenticate(_setting.Username, _setting.Password);
            _smtpClient.Send(message);
            _smtpClient.Disconnect(true);
        }
        catch (Exception e)
        {
            _logger.Error($"[SendEmail] Failed to send email : {e.Message}");
        }
        finally
        {
            _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }
    }

    public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        var message = SetupEmailMessage(request);
        try
        {
            await _smtpClient.ConnectAsync(_setting.SmtpServer, _setting.Port, _setting.UseSsl, cancellationToken);
            await _smtpClient.AuthenticateAsync(_setting.Username, _setting.Password, cancellationToken);
            await _smtpClient.SendAsync(message);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.Error($"[SendEmailAsync] Failed to send email : {e.Message}");
        }
        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
            _smtpClient.Dispose();
        }
    }

    private MimeMessage SetupEmailMessage(MailRequest request)
    {
        var message = new MimeMessage()
        {
            Sender = new MailboxAddress(_setting.DisplayName, _setting.From),
            Subject = request.Subject,
            Body = new BodyBuilder
            {
                HtmlBody = request.Body,
            }.ToMessageBody(),
            
        };

        if (request.ToAddresses.Any())
        {
            foreach (var address in request.ToAddresses)
            {
                message.To.Add(MailboxAddress.Parse(address));
            }
        }
        else
        {
            var toAddress = request.ToAddress;
            message.To.Add(MailboxAddress.Parse(toAddress));
        }

        return message;
    }
}