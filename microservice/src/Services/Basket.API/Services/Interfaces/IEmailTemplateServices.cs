﻿namespace Basket.API.Services.Interfaces;

public interface IEmailTemplateServices
{
    string GenerateReminderCheckoutOrderEmail(string username);
}
