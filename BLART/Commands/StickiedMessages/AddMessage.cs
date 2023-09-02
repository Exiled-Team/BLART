namespace BLART.Commands.StickiedMessages;

using BLART.Services;
using Discord.Interactions;
using Discord.WebSocket;

[Group("stick", "Commands to manage channel sticky messages.")]
public partial class StickiedMessages : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add", "Adds a sticky message to a channel.")]
    public async Task Add([Summary("channel", "The channel to stick a message to.")]SocketTextChannel channel, [Summary("message", "The message to stick.")]string message)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            await RespondAsync("Sticky message cannot be empty!", ephemeral: true);
            return;
        }

        var sticky = DatabaseHandler.GetStickyMessage(channel.Id);
        if (sticky != null)
        {
            await RespondAsync($"The {channel.Mention} channel already has an active sticky message! Remove it first.", ephemeral: true);
            return;
        }

        DatabaseHandler.AddEntry(channel.Id, message, DatabaseType.StickiedMessage, Context.User.Id);
        await RespondAsync($"The following sticky message has been added to the {channel.Mention} channel.", ephemeral: true);

    }
}
