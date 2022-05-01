namespace BLART.Commands.RedRoles;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("redrole", "Commands for managing red roles.")]
public partial class RedRoleCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("give", "Gives the specified user the red role.")]
    public async Task Give([Summary("User", "The user to give the role to.")] SocketUser user, [Summary("Reason", "The reason why they are getting the role.")][Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User, true))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        IGuildUser target = (IGuildUser)user;
        IRole role = Context.Guild.GetRole(Program.Config.RedRoleId);

        if (target.RoleIds.Any(r => r == role.Id))
        {
            await RespondAsync("This user already has the red role.");
            return;
        }

        await target.AddRoleAsync(role);
        DatabaseHandler.AddEntry(target.Id, reason, DatabaseType.RedRole, Context.User.Id);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Red Role Issued",
            $"{target.Username} has been issued a red role.", Color.Red));
    }
}