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
    public ulong ContributorId { get; set; }
    public string NorthwoodApiKey { get; set; }
    public ulong StaffChannelId { get; set; }
    public string SqlUser { get; set; }
    public string SqlPassword { get; set; }
    public string SqlDatabase { get; set; }
    public string SqlServer { get; set; }

    public List<ulong> CreditRoleIds { get; set; } = new();

    public static readonly Config Default = new()
    {
        BotToken = Environment.GetEnvironmentVariable("BOT_TOKEN")!,
        BotPrefix = Environment.GetEnvironmentVariable("BOT_PREFIX")!,
        DiscStaffId = ulong.Parse(Environment.GetEnvironmentVariable("DISC_STAFF_ID")!),
        SpamLimit = int.Parse(Environment.GetEnvironmentVariable("SPAM_LIMIT")!),
        SpamTimeout = int.Parse(Environment.GetEnvironmentVariable("SPAM_TIMEOUT")!),
        ChannelRentId = ulong.Parse(Environment.GetEnvironmentVariable("CHANNEL_RENT_ID")!),
        ChannelRentCatId = ulong.Parse(Environment.GetEnvironmentVariable("CHANNEL_RENT_CATEGORY_ID")!),
        LogsId = ulong.Parse(Environment.GetEnvironmentVariable("LOGS_CHANNEL_ID")!),
        Debug = bool.Parse(Environment.GetEnvironmentVariable("DEBUG")!),
        RedRoleId = ulong.Parse(Environment.GetEnvironmentVariable("RED_ROLE_ID")!),
        BugReportId = ulong.Parse(Environment.GetEnvironmentVariable("BUG_REPORT_CHANNEL_ID")!),
        TriggerLengthLimit = int.Parse(Environment.GetEnvironmentVariable("LENGHT_LIMIT")!),
        ContributorId = ulong.Parse(Environment.GetEnvironmentVariable("CONTRIBUTOR_ROLE_ID")!),
        NorthwoodApiKey = Environment.GetEnvironmentVariable("NW_API_KEY")!,
        StaffChannelId = ulong.Parse(Environment.GetEnvironmentVariable("STAFF_CHANNEL_ID")!),
        SqlServer = Environment.GetEnvironmentVariable("DB_SERVER")!,
        SqlUser = Environment.GetEnvironmentVariable("DB_USER")!,
        SqlPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")!,
        SqlDatabase = Environment.GetEnvironmentVariable("DB_DATABASE")!,
        CreditRoleIds = GetCreditRoleIds().ToList()
    };

    private static IEnumerable<ulong> GetCreditRoleIds()
    {
        var env = Environment.GetEnvironmentVariable("CREDIT_ROLE_IDS")!;
        var roles = env.Split(',');
        foreach (var role in roles)
        {
            if (ulong.TryParse(role, out ulong returnValue))
                yield return returnValue;
        }
    }
}