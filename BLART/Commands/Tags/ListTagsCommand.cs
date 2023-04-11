namespace BLART.Commands.Tags;

using BLART.Services;

using Discord;
using Discord.Interactions;

public partial class TagCommands
{
    [SlashCommand("list", "Lists all current tag names.")]
    public async Task ListTags()
    {
        string message = string.Empty;

        foreach (string tagName in DatabaseHandler.GetTagNames())
            message += $"{tagName}\n";

        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Tag list", message, Color.Purple), ephemeral: true);
    }
}