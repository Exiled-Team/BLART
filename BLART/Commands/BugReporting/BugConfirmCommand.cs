namespace BLART.Commands.BugReporting;

using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Modules;
using Services;

public class BugConfirmCommand : ModuleBase<SocketCommandContext>
{
    [Command("confirm")]
    [Summary("Confirms your bug report.")]
    public async Task Confirm()
    {
        if (!BugReporting.BugReports.ContainsKey(Context.Message.Author))
        {
            await ReplyAsync("You do not currently have a bug report awaiting confirmation.");
            return;
        }

        (List<FileAttachment> attachments, EmbedBuilder builder) = BugReporting.BugReports[Context.Message.Author];

        IMessage message;
        if (attachments.Count > 0)
            message = await BugReporting.BugReportChannel.SendFilesAsync(attachments, string.Empty, embed: builder.Build());
        else
            message = await BugReporting.BugReportChannel.SendMessageAsync(embed: builder.Build());

        BugReporting.BugReports.Remove(Context.Message.Author);
        BugReporting.OpenThreads.Add(message.Id, await BugReporting.BugReportChannel.CreateThreadAsync(builder.Description[..23], message: message, autoArchiveDuration: ThreadArchiveDuration.ThreeDays, invitable: true));
        DatabaseHandler.AddEntry(message.Id, BugReporting.OpenThreads[message.Id].Id.ToString(), DatabaseType.BugReport);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
    }
}