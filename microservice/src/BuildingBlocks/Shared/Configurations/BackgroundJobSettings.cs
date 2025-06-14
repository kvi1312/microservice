namespace Shared.Configurations;

public class BackgroundJobSettings
{
    public string HangfireUrl {  get; set; } = string.Empty;
    public string ApiGwUrl {  get; set; } = string.Empty;
    public string BasketUrl {  get; set; } = string.Empty;
    public string ScheduledJobUrl {  get; set; } = string.Empty;
}
