namespace BLART.Commands.Tags;

using BLART.Modals;
using BLART.Services;

using Discord;
using Discord.Commands;
using Discord.Interactions;

[Discord.Interactions.Group("tag", "Commands for managing tags.")]
public partial class TagCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add", "Adds a new tag.")]
    public async Task AddTag()
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User, true))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        await RespondWithModalAsync(TagModal.CreateTagModal());
    }
}