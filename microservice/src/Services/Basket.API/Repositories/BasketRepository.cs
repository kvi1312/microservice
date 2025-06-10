using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOS.ScheduledJob;
using ILogger = Serilog.ILogger;
namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;
        private readonly BackgroundJobHttpServices _backgroundJobHttpServices;
        private readonly IEmailTemplateServices _emailTemplateServices;
        public BasketRepository(IDistributedCache redisCacheService, ILogger logger, ISerializeService serializeService, IEmailTemplateServices emailTemplateServices, BackgroundJobHttpServices backgroundJobHttpServices)
        {
            _redisCacheService = redisCacheService;
            _logger = logger;
            _serializeService = serializeService;
            _emailTemplateServices = emailTemplateServices;
            _backgroundJobHttpServices = backgroundJobHttpServices;
        }

        public async Task<bool> DeletedBasketFromUserName(string userName)
        {
            try
            {
                _logger.Information("[BEGIN] Deleting basket for user {UserName}", userName);
                await _redisCacheService.GetStringAsync(userName);
                _logger.Information("[END] Deleting basket for user {UserName}", userName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting basket for user {UserName}", userName);
                return false;
            }
        }

        public async Task<Cart?> GetBasketByUserName(string userName)
        {
            _logger.Information("[BEGIN] Getting basket for user {UserName}", userName);
            var basket = await _redisCacheService.GetStringAsync(userName);
            _logger.Information("[END] Getting basket for user {UserName}", userName);
            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            if (options is not null)
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart), options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
            }

            try
            {
                await TriggerSendReminderEmailAsync(cart);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return await GetBasketByUserName(cart.UserName);
        }

        private async Task TriggerSendReminderEmailAsync(Cart cart)
        {
            var emailTemplate = _emailTemplateServices.GenerateReminderCheckoutOrderEmail(cart.UserName);
            var model = new ReminderCheckoutOrderDto(cart.EmailAddress, "Reminder Checkout", emailTemplate, DateTimeOffset.UtcNow.AddSeconds(30));
            const string uri = "/api/scheduled-jobs/send-email-reminder-checkout-order";
            var response = await _backgroundJobHttpServices.Client.PostAsJsonAsync<ReminderCheckoutOrderDto>(uri, model);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                var jobId = await response.ReadContentAs<string>();
                if (!string.IsNullOrEmpty(jobId))
                {
                    // Handle when user update basket and need job id to be tracked and update email into newest version
                    _logger.Information("Scheduled job created successfully with ID: {JobId}", jobId);
                    cart.JobId = jobId;
                    await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
                }
            }
        }
    }
}
