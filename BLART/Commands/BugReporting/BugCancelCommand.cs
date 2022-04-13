namespace BLART.Commands.BugReporting;

using Discord;
using Discord.Commands;
using Modules;

public class BugCancelCommand : ModuleBase<SocketCommandContext>
{
    [Command("cancel")]
    [Summary("Cancels your pending bug report.")]
    public async Task Cancel()
    {
        if (!BugReporting.BugReports.ContainsKey(Context.Message.Author))
        {
            await ReplyAsync("You do not currently have a pending bug report.");
            return;
        }

        BugReporting.BugReports.Remove(Context.Message.Author);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
    }
}