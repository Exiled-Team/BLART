namespace BLART.Commands.RedRoles;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class GiveCommand : ModuleBase<SocketCommandContext>
{
    [Command("redrole")]
    [Summary("Gives the specified user the red role.")]
    public async Task Give([Summary("The user to give the role to.")] SocketUser user, [Summary("The reason why they are getting the role.")][Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        IGuildUser target = (IGuildUser)user;
        IRole role = Context.Guild.GetRole(Program.Config.RedRoleId);

        if (target.RoleIds.Any(r => r == role.Id))
        {
            await ReplyAsync("This user already has the red role.");
            return;
        }

        await target.AddRoleAsync(role);
        DatabaseHandler.AddEntry(target.Id, reason, DatabaseType.RedRole, Context.Message.Author.Id);
    }
}