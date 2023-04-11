namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Services;

[Group("role", "Commands to manage self-roles")]
public partial class RoleCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add", "Adds a role as a self-assignable role.")]
    public async Task AddRole([Summary("Role", "The role to add")] IRole role)
    {
        await DeferAsync(ephemeral: true);
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await FollowupAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            
            return;
        }

        int highestPosition = 0;
        foreach (ulong roleId in ((IGuildUser)Context.User).RoleIds)
        {
            SocketRole userRole = Context.Guild.GetRole(roleId);
            if (userRole.Position > highestPosition)
                highestPosition = userRole.Position;
        }

        if (role.Position > highestPosition)
        {
            await FollowupAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            
            return;
        }

        DatabaseHandler.AddEntry(role.Id, string.Empty, DatabaseType.SelfRole);
        await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Self-Role Added", $"The role {role.Name} has been added as a self-assignable role.", Color.Green), ephemeral: true);
    }
}