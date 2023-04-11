#pragma warning disable CS8618
namespace BLART;

public class Config
{		
    public string BotPrefix { get; set; }
    public string BotToken { get; set; }
    public ulong DiscStaffId { get; set; }
    public int SpamLimit { get; set; }
    public int SpamTimeout { get; set; }
    public ulong ChannelRentId { get; set; }
    public ulong ChannelRentCatId { get; set; }
    public ulong LogsId { get; set; }
    public bool Debug { get; set; }
    public ulong RedRoleId { get; set; }
    public ulong BugReportId { get; set; }
    public int TriggerLengthLimit { get; set; }
    public ulong ContributorId { get; set; } = 668651927298375690;
    public string NorthwoodApiKey { get; set; }
    public ulong StaffChannelId { get; set; }
    public string SqlUser { get; set; }
    public string SqlPassword { get; set; }
    public string SqlDatabase { get; set; }

    public static readonly Config Default = new()
    {
        BotToken = string.Empty,
        BotPrefix = "~",
        DiscStaffId = 0,
        SpamLimit = 10,
        SpamTimeout = 5,
        ChannelRentId = 0,
        ChannelRentCatId = 0,
        LogsId = 0,
        Debug = false,
        RedRoleId = 0,
        BugReportId = 0,
        TriggerLengthLimit = 200,
        ContributorId = 0,
        NorthwoodApiKey = string.Empty,
        StaffChannelId = 0,
        SqlUser = string.Empty,
        SqlPassword = string.Empty,
        SqlDatabase = string.Empty,
    };
}