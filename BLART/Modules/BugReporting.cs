namespace BLART.Modules;

using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Modals;
using Services;

public class BugReporting
{
    private static SocketTextChannel? _channel = null;

    public static SocketTextChannel BugReportChannel => _channel ??= Bot.Instance.Guild.GetTextChannel(Program.Config.BugReportId);

    public static Dictionary<IUser, EmbedBuilder> BugReports = new();

    public static Dictionary<ulong, SocketThreadChannel> OpenThreads = new();

    public static async Task LoadDatabaseEntries()
    {
        foreach (SocketThreadChannel thread in BugReportChannel.Threads)
        {
            Log.Debug(nameof(LoadDatabaseEntries), $"Getting message ID for thread {thread.Id}");
            ulong messageId = DatabaseHandler.GetMessageId(thread.Id);
            IUserMessage message = (IUserMessage) await BugReportChannel.GetMessageAsync(messageId);
            if (message is null)
            {
                DatabaseHandler.RemoveEntry(messageId, DatabaseType.BugReport);
                return;
            }

            Log.Debug(nameof(LoadDatabaseEntries), $"Messaged ID for {thread.Id} found: {messageId}");
            if (messageId != 0) 
                OpenThreads.Add(messageId, thread);

            Log.Debug(nameof(LoadDatabaseEntries), "Adding context buttons to old message.");
            if (message.Components.Count == 0)
                await message.ModifyAsync(x =>
                {
                    x.Components = BugReportModal.StaffButtons(messageId);
                });
        }
    }
}