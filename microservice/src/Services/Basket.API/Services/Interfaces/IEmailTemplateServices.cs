namespace Basket.API.Services.Interfaces;

public interface IEmailTemplateServices
{
    string GenerateReminderCheckoutOrderEmail(string email, string username);
}
