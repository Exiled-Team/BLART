namespace BLART.Commands.BugReporting;

using Discord;
using Discord.Commands;
using Discord.Rest;
using Modules;
using Services;

public class BugReportCommand : ModuleBase<SocketCommandContext>
{
    [Command("bug")]
    [Summary("Reports a bug in EXILED.")]
    public async Task Bug([Summary("The version of EXILED being used.")] string exiledVersion, [Summary("The description of the bug")] [Remainder] string description)
    {
        EmbedBuilder builder = new();
        builder.WithAuthor(Context.Message.Author);
        builder.WithColor(Color.Red);
        builder.WithCurrentTimestamp();
        builder.WithFooter(EmbedBuilderService.Footer);
        builder.WithTitle("Bug Report");
        builder.AddField("Exiled Version", exiledVersion);
        builder.WithDescription(description);
        IReadOnlyCollection<Attachment>? attachments = Context.Message.Attachments;
        List<FileAttachment> attachmentList = new();
        
        if (attachments is not null)
        {
            foreach (Attachment attachment in attachments)
            {
                using HttpClient client = new();
                attachmentList.Add(new FileAttachment(await client.GetStreamAsync(attachment.Url),
                    attachment.Filename));
            }
        }

        try
        {
            BugReporting.BugReports.Add(Context.Message.Author, new(attachmentList, builder));
        }
        catch (Exception e)
        {
            await ReplyAsync("Failed to cache bug report. Report this to Joker.");
            Log.Error(nameof(Bug), e);
        }

        await ReplyAsync("This comment is specifically for reporting **EXILED** bugs only. If you are certain this bug is caused by EXILED or it's API and not by a plugin, please `~confirm`.\nTo cancel this report use `~cancel`");
    }
}