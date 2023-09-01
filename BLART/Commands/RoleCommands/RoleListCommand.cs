namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Services;

public partial class RoleCommands
{
    [SlashCommand("list", "Lists available self-assignable roles.")]
    public async Task ListRoles()
    {
        Dictionary<string, string> roles = new Dictionary<string, string>
        {
            ["Plugin Roles"] = "## Plugin Roles\n",
            ["Misc Roles"] = "## Miscellaneous Roles\n"
        };

        await DeferAsync(ephemeral: true);
        
        List<ulong> roleIds = DatabaseHandler.GetSelfRoles();
        EmbedBuilder builder = new();
        builder.WithTitle("Self-Assignable Roles");
        builder.WithFooter(EmbedBuilderService.Footer);
        builder.WithCurrentTimestamp();
        foreach (ulong roleId in roleIds)
        {
            var role = Context.Guild.Roles.FirstOrDefault(role => role.Id == roleId);

            if (role is null)
                continue;

            var channel = Context.Guild.TextChannels.FirstOrDefault(channel => channel.Name.ToLower() == role.Name.ToLower());
            if (channel is not null)
            {
                roles["Plugin Roles"] += $"<@&{roleId}> - Role for <#{channel.Id}>.\n";
            }
            else
            {
                roles["Misc Roles"] += $"<@&{roleId}>\n";
            }
        }
        builder.WithDescription(roles["Plugin Roles"] + "\n" + roles["Misc Roles"]);
        builder.WithColor(Color.Green);

        await FollowupAsync(embed: builder.Build(), ephemeral: true);
    }
}