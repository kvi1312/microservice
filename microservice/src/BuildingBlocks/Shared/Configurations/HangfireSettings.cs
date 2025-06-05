namespace Shared.Configurations;

public class HangfireSettings
{
    public string Route { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public DashboardSetting Dashboard { get; set; } = new();
    public DatabaseSettings Storage { get; set; } = new();
}


public class DashboardSetting
{
    public string AppPath { get; set; } = string.Empty;
    public int StatsPollingInterval { get; set; }
    public string DashboardTitle { get; set; } = string.Empty;
}