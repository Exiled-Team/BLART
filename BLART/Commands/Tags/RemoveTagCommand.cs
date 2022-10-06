namespace BLART.Commands.Tags;

using BLART.Objects;
using BLART.Services;

using Discord;
using Discord.Interactions;

public partial class TagCommands
{
    [SlashCommand("remove", "Removes a specified tag")]
    public async Task Remove([Summary("Name", "The tag name to be removed.")] string name)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User, true))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        Tag? tag = DatabaseHandler.GetTag(name);

        if (tag is null)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.NoRecordFound), ephemeral: true);
            return;
        }

        DatabaseHandler.RemoveEntry(tag.Id, DatabaseType.Tags);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Tag removed", "The specified tag has been removed.", Color.Green), ephemeral: true);
    }
}