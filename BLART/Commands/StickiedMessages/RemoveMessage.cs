namespace BLART.Commands.StickiedMessages;

using BLART.Services;
using Discord;
using Discord.Interactions;

public partial class StickiedMessages : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("remove", "Removes a sticky message from a channel.")]
    public async Task Remove([Summary("channel", "The channel to remove a stuck message from.")] ITextChannel channel)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        var sticky = DatabaseHandler.GetStickyMessage(channel.Id);
        if (sticky == null)
        {
            await RespondAsync($"The {channel.Mention} channel does not have a sticky message.", ephemeral: true);
            return;
        }

        DatabaseHandler.RemoveEntry(channel.Id, DatabaseType.StickiedMessage);
        await RespondAsync($"Sticky message has been removed from the {channel.Mention} channel!", ephemeral: true);
    }
}
