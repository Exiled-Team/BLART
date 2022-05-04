namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Services;

[Group("role", "Commands to manage self-roles")]
public partial class RoleCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add", "Adds a role as a self-assignable role.")]
    public async Task AddRole([Summary("Role", "The role to add")] IRole role)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User, true))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            
            return;
        }
        
        DatabaseHandler.AddEntry(role.Id, string.Empty, DatabaseType.SelfRole);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Self-Role Added", $"The role {role.Name} has been added as a self-assignable role.", Color.Green), ephemeral: true);
    }
}