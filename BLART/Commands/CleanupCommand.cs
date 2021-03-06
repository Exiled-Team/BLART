namespace BLART.Commands;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public class CleanupCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("cleanup", "Cleans up messages in a given channel.")]
    public async Task Cleanup(
        [Summary("Channel", "The channel to clean up in.")] SocketTextChannel channel,
        [Summary("Amount", "The amount of messages to delete.")] int amount,
        [Summary("MessageID", "The message ID to start deleting at. (Optional)")] string id = "0",
        [Summary("Direction", "The direction to move in. (Optional)")] Direction direction = Direction.Before)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        if (!ulong.TryParse(id, out ulong userId))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId, id),
                ephemeral: true);
            return;
        }

        IEnumerable<IMessage> messages = null!;
        if (userId != 0)
            messages = await channel.GetMessagesAsync(userId, direction, amount).FlattenAsync();
        else
            messages = await channel.GetMessagesAsync(amount).FlattenAsync();

        await channel.DeleteMessagesAsync(messages);
    }
}