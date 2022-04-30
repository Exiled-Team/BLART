namespace BLART.Commands.RedRoles;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class RemoveCommand : ModuleBase<SocketCommandContext>
{
    [Command("remredrole")]
    [Summary("Removes the red role from the specified user.")]
    public async Task Remove([Summary("The user who's red role is to be removed.")] SocketUser user)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        IGuildUser target = (IGuildUser)user;
        IRole role = Context.Guild.GetRole(Program.Config.RedRoleId);

        if (target.RoleIds.All(r => r != role.Id))
        {
            await ReplyAsync("This user does not have a red role.");
            return;
        }

        await target.RemoveRoleAsync(role);
        DatabaseHandler.RemoveEntry(target.Id, DatabaseType.RedRole);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
    }
}