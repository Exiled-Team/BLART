namespace BLART.Modules;

using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Services;

public class BugReporting
{
    private static SocketTextChannel? _channel = null;

    public static SocketTextChannel BugReportChannel => _channel ??= Bot.Instance.Guild.GetTextChannel(Program.Config.BugReportId);

    public static Dictionary<IUser, EmbedBuilder> BugReports = new();

    public static Dictionary<ulong, SocketThreadChannel> OpenThreads = new();

    public static Task LoadDatabaseEntries()
    {
        foreach (SocketThreadChannel thread in BugReportChannel.Threads)
        {
            Log.Debug(nameof(LoadDatabaseEntries), $"Getting message ID for thread {thread.Id}");
            ulong messageId = DatabaseHandler.GetMessageId(thread.Id);
            Log.Debug(nameof(LoadDatabaseEntries), $"Messaged ID for {thread.Id} found: {messageId}");
                if (messageId != 0)
                    OpenThreads.Add(messageId, thread);
        }

        return Task.CompletedTask;
    }
}