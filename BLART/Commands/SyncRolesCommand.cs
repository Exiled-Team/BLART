namespace BLART.Commands;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Modals;
using Services;

public class SyncRolesCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("syncroles", "Re-adds missing roles to self-assigned roles.")]
    public async Task SyncRoles()
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied),
                ephemeral: true);
            return;
        }
        
        List<ulong> roleIds = DatabaseHandler.GetSelfRoles();
        foreach (SocketRole role in Bot.Instance.Guild.Roles)
        {
            if (!roleIds.Contains(role.Id))
                SyncRolesModal.Roles.Add(role.Id);
        }

        ulong roleId = SyncRolesModal.Roles.FirstOrDefault();
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Select Assignability", $"Is <@&{roleId}> self-assignable?", Color.Orange), ephemeral: true, components: SyncRolesModal.SyncComponent(roleId));
    }
}