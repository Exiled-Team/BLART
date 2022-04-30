namespace BLART.Commands.BugReporting;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Modules;
using Services;

public class BugDuplicateCommand : ModuleBase<SocketCommandContext>
{
    [Command("duplicate")]
    [Summary("Marks a bug as a duplicate of another.")]
    public async Task Duplicate([Summary("The message of the duplicate bug.")] ulong messageId, [Summary("The original bug message.")] ulong originalMessageId)
    {
        IMessage message = await BugReporting.BugReportChannel.GetMessageAsync(messageId);
        IMessage originalMessage = await BugReporting.BugReportChannel.GetMessageAsync(originalMessageId);
        if ($"{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}" != message.Embeds.First().Author!.Value.Name && !CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied, "Only the bug submitter, Discord Staff and EXILED Developers can mark reports as duplicates."));
            return;
        }

        bool flag = false;
        if (!BugReporting.OpenThreads.ContainsKey(message.Id))
        {
            Log.Warn(nameof(Duplicate), "Unable to find duplicate thread.");
            flag = true;
        }

        if (!BugReporting.OpenThreads.ContainsKey(originalMessage.Id))
        {
            Log.Warn(nameof(Duplicate), "Unable to find original thread.");
            flag = true;
        }

        if (flag)
        {
            await ReplyAsync("An error has occured while making a report as a duplicate.");
            return;
        }

        SocketThreadChannel duplicateThread = BugReporting.OpenThreads[message.Id];
        SocketThreadChannel thread = BugReporting.OpenThreads[originalMessage.Id];

        await duplicateThread.JoinAsync();
        await duplicateThread.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Duplicate Report",
            $"This thread has been marked as a duplicate of <#{thread.Id}> and has been locked.", Color.Gold));
        await duplicateThread.ModifyAsync(x =>
        {
            x.Locked = true;
            x.Archived = true;
        });
        
        await message.DeleteAsync();
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
    }
}