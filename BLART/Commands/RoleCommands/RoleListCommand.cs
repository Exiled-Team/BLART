namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Services;

public partial class RoleCommands
{
    [SlashCommand("list", "Lists available self-assignable roles.")]
    public async Task ListRoles()
    {
        await DeferAsync(ephemeral: true);
        
        List<ulong> roleIds = DatabaseHandler.GetSelfRoles();
        EmbedBuilder builder = new();
        builder.WithTitle("Self-Assignable Roles");
        builder.WithFooter(EmbedBuilderService.Footer);
        builder.WithCurrentTimestamp();
        string description = string.Empty;
        foreach (ulong roleId in roleIds)
            description += $"<@&{roleId}>\n";
        builder.WithDescription(description);
        builder.WithColor(Color.Green);

        await FollowupAsync(embed: builder.Build(), ephemeral: true);
    }
}