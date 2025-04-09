﻿using Contracts.Configurations;

namespace Infrastructure.Configurations;

public class SMTPEmailSettings : IEmailSettings
{
    public string DisplayName { get; set; }
    public bool EnableVerification { get; set; }
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public bool UseSsl { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}