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
    }
}