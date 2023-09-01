namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Services;

public partial class RoleCommands
{
    [SlashCommand("assign", "Assigns or removes a role to/from yourself.")]
    public async Task AssignRole([Summary("Role", "The role to give/remove to/from yourself.")] IRole role)
    {
        if (DatabaseHandler.GetSelfRoles().Any(r => r == role.Id))
        {
            IGuildUser user = (IGuildUser)Context.User;
            if (user.RoleIds.Any(r => r == role.Id))
            {
                await user.RemoveRoleAsync(role, new() { AuditLogReason = $"Removed self-assignable role." });
                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Role removed", "The selected role has been removed.", Color.Green), ephemeral: true);
            }
            else
            {
                await user.AddRoleAsync(role, new() { AuditLogReason = "Added self-assignable role." });
                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Role Added", "The selected role has been added.", Color.Green), ephemeral: true);
            }
        }
        else
            await RespondAsync(
                embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied,
                    "The selected role is not self-assignable."), ephemeral: true);
    }
}