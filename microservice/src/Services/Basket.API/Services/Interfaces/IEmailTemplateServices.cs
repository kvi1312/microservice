namespace Basket.API.Services.Interfaces;

public interface IEmailTemplateServices
{
    string GenerateReminderCheckoutOrderEmail(string username, string checkoutUrl = "basket/checkout");
}
