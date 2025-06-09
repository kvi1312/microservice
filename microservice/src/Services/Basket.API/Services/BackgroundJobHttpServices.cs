using Shared.Configurations;

namespace Basket.API.Services;

public class BackgroundJobHttpServices
{
    public HttpClient Client { get; }
    public BackgroundJobHttpServices(HttpClient client, BackgroundJobSettings backgroundJobSettings)
    {
        client.BaseAddress = new Uri(backgroundJobSettings.HangfireUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("accept", "application/json");
        Client = client;
    }
}
