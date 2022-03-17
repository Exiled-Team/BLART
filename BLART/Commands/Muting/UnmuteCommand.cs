namespace BLART.Commands.Muting;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class UnmuteCommand : ModuleBase<SocketCommandContext>
{
    [Command("unmute")]
    [Summary("Unmutes the specified user.")]
    public async Task Unmute([Summary("The user to unmute")] SocketUser user)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        await Logging.SendLogMessage("User unmuted", $"{Context.Message.Author.Username} unmuted {user.Username}.", Color.Orange);
        await ((IGuildUser)user).RemoveTimeOutAsync();
        await ReplyAsync("User unmuted.");
    }
}