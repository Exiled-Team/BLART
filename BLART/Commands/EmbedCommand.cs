namespace BLART.Commands;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Modals;
using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("embed", "Commands for managing embeded messages.")]
public partial class EmbedCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("send", "Sends an embeded message into a specific channel.")]
    public async Task Embed([Summary("Channel", "The channel to send the embed to.")] ITextChannel channel)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        await RespondWithModalAsync(EmbedModal.Embed(channel.Id));
    }
}