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
        [Discord.Commands.Summary("The channel to clean up in.")] SocketTextChannel channel,
        [Discord.Commands.Summary("The amount of messages to delete.")] int amount,
        [Discord.Commands.Summary("The message ID to start deleting at. (Optional)")] ulong messageId = 0,
        [Discord.Commands.Summary("The direction to move in. (Optional)")] Direction direction = Direction.Before)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        IEnumerable<IMessage> messages = null!;
        if (messageId != 0)
            messages = await channel.GetMessagesAsync(messageId, direction, amount).FlattenAsync();
        else
            messages = await channel.GetMessagesAsync(amount).FlattenAsync();

        await channel.DeleteMessagesAsync(messages);
    }
}