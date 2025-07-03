using Infrastructure.Extensions;
using Shared.Configurations;
using Shared.DTOS.ScheduledJob;

namespace Basket.API.Services;

public class BackgroundJobHttpServices
{
    private readonly HttpClient _client;
    private readonly string _scheduledJobUrl;

    public BackgroundJobHttpServices(HttpClient client, BackgroundJobSettings backgroundJobSettings)
    {
        client.BaseAddress = new Uri(backgroundJobSettings.HangfireUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("accept", "application/json");
        _client = client;
        _scheduledJobUrl = backgroundJobSettings.ScheduledJobUrl;
    }

    public async Task<string> SendEmailReminderCheckout(ReminderCheckoutOrderDto dto)
    {
        var uri = $"{_scheduledJobUrl}/send-email-reminder-checkout-order";
        var response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, uri, dto);
        var jobId = string.Empty;
        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            jobId = await HttpClientExtension.ReadContentAs<string>(response);
        return jobId;
    }

    public void DeleteReminderCheckoutOrder(string jobId)
    {
        var uri = $"{_scheduledJobUrl}/delete/jobId/{jobId}";
        _client.DeleteAsync(uri);
    }
}