namespace BLART.Commands.Tags;

using BLART.Modals;
using BLART.Objects;
using BLART.Services;

using Discord;
using Discord.Commands;
using Discord.Interactions;

public partial class TagCommands
{
    [SlashCommand("update", "Updates an existing tag.")]
    public async Task UpdateTag([Discord.Interactions.Summary("Name", "The name of the tag to be updated.")] string name)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User, true))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        await RespondWithModalAsync(TagModal.EditTagModal(name));
    }
}