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

        // Remove existing message
        string? id = DatabaseHandler.GetStickyMessageID(channel.Id);
        if (id is not null)
        {
            await DeferAsync(true);

            IMessage msg = await channel.GetMessageAsync(ulong.Parse(id));

            if (msg is not null)
                await msg.DeleteAsync(new() { AuditLogReason = "Removed message for deleted sticky message." });
        }

        DatabaseHandler.RemoveEntry(channel.Id, DatabaseType.StickiedMessage);
        await RespondAsync($"Sticky message has been removed from the {channel.Mention} channel!", ephemeral: true);
    }
}
