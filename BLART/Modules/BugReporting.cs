namespace BLART.Modules;

using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Services;

public class BugReporting
{
    private static SocketTextChannel? _channel = null;

    public static SocketTextChannel BugReportChannel => _channel ??= Bot.Instance.Guild.GetTextChannel(Program.Config.BugReportId);

    public static Dictionary<IUser, (List<FileAttachment>, EmbedBuilder)> BugReports = new();

    public static Dictionary<ulong, SocketThreadChannel> OpenThreads = new();

    public static async Task LoadDatabaseEntries()
    {
        foreach (SocketThreadChannel thread in BugReportChannel.Threads)
        {
            ulong messageId = DatabaseHandler.GetMessageId(thread.Id);
                if (messageId != 0)
                    OpenThreads.Add(messageId, thread);
        }
    }
}