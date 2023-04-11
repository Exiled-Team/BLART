namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Services;

public partial class RoleCommands
{
    [SlashCommand("remove", "Removes a role from being self-assignable.")]
    public async Task RemoveRole([Summary("Role", "The role to remove")] IRole role)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            
            return;
        }

        DatabaseHandler.RemoveEntry(role.Id, DatabaseType.SelfRole);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Self-Role Removed", $"The role {role.Name} has been removed as a self-assignable role.", Color.Green), ephemeral: true);
    }
}