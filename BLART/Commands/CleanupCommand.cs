namespace BLART.Commands;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class CleanupCommand : ModuleBase<SocketCommandContext>
{
    [Command("cleanup")]
    [Summary("Cleans up messages in a given channel.")]
    public async Task Cleanup(
        [Summary("The channel to clean up in.")] SocketTextChannel channel,
        [Summary("The amount of messages to delete.")] int amount,
        [Summary("The message ID to start deleting at. (Optional)")] ulong messageId = 0,
        [Summary("The direction to move in. (Optional)")] Direction direction = Direction.Before)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        IEnumerable<IMessage> messages = null!;
        if (messageId != 0)
            messages = await channel.GetMessagesAsync(messageId, direction, amount).FlattenAsync();
        else
            messages = await channel.GetMessagesAsync(amount).FlattenAsync();

        await channel.DeleteMessagesAsync(messages);
        await ReplyAsync("Messages deleted.");
    }
}