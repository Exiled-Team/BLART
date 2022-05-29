namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Services;

public partial class RoleCommands
{
    [SlashCommand("ping", "Pings a role.")]
    public async Task Ping([Summary("role", "The role to ping.")] IRole role)
    {
        if (((IGuildUser) Context.User).RoleIds.All(r => r != 656673780332101648))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        if (DatabaseHandler.GetSelfRoles().Any(r => r == role.Id))
            await RespondAsync(role.Mention);
        else
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Role Ping",
                $"Unable to ping {role.Name}, as it is not a self-assignable role.", Color.Red), ephemeral: true);
    }
}