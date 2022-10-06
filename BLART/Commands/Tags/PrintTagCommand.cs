namespace BLART.Commands.Tags;

using BLART.Objects;
using BLART.Services;

using Discord;
using Discord.Interactions;

public partial class TagCommands
{
    [SlashCommand("print", "Prints the specified tag as an embeded message in the current channel.")]
    public async Task PrintTag([Summary("Name", "The name of the tag to send")] string name)
    {
        Tag? tag = DatabaseHandler.GetTag(name);

        if (tag is null)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.NoRecordFound), ephemeral: true);
            return;
        }

        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed(tag.Name, tag.Text, Color.Blue));
    }
}