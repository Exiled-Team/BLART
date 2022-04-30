namespace BLART.Commands.BugReporting;

using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Modules;
using Services;

public class BugSolveCommand : ModuleBase<SocketCommandContext>
{
    [Command("solve")]
    [Summary("Marks a bug as resolved.")]
    public async Task Solve([Summary("The bug report to solve.")] ulong messageId)
    {
        Log.Debug(nameof(Solve), "Running command.");
        IMessage message = await BugReporting.BugReportChannel.GetMessageAsync(messageId);
        if ($"{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}" != message.Embeds.First().Author!.Value.Name && !CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied, "Only the bug submitter, Discord Staff and EXILED Developers can mark reports as solved."));
            return;
        }

        SocketUserMessage msg = (message as SocketUserMessage)!;

        IEmbed embed = msg.Embeds.First();
        await msg?.ModifyAsync(x =>
        {
            EmbedBuilder builder = new();
            builder.WithTitle($"Marked SOLVED by: {Context.Message.Author.Username}");
            builder.WithDescription(embed.Description);
            builder.WithCurrentTimestamp();
            builder.WithFooter(embed.Footer!.Value.Text);
            builder.WithColor(Color.Gold);
            builder.Fields.Add(new EmbedFieldBuilder
            {
                IsInline = false,
                Name = "Exiled Version",
                Value = embed.Fields.First().Value,
            });
            x.Embed = builder.Build();
        })!;

        await BugReporting.OpenThreads[message.Id].ModifyAsync(x => x.Archived = true);
        DatabaseHandler.RemoveEntry(message.Id, DatabaseType.BugReport);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
    }
}