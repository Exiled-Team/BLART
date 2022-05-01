namespace BLART.Commands.RedRoles;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class RedRoleCommands
{
    [SlashCommand("remove", "Removes the users red role.")]
    public async Task Remove([Summary("User", "The user who's red role is to be removed.")] SocketUser user)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User, true))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        IGuildUser target = (IGuildUser)user;
        IRole role = Context.Guild.GetRole(Program.Config.RedRoleId);

        if (target.RoleIds.All(r => r != role.Id))
        {
            await RespondAsync("This user does not have a red role.");
            return;
        }

        await target.RemoveRoleAsync(role);
        DatabaseHandler.RemoveEntry(target.Id, DatabaseType.RedRole);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Red role removed",
            $"{target.Username} has had their red role removed.", Color.Green));
    }
}