namespace BLART.Commands.Warns;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class WarnCommand : ModuleBase<SocketCommandContext>
{
    [Command("warn")]
    [Summary("Warns the user.")]
    public async Task Warn(
        [Summary("The user to warn")] SocketUser user,
        [Summary("The reason for the warning.")] [Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        reason = ReasonParsing.ParseRules(reason);
        try
        {
            await user.SendMessageAsync(
                $"You have been warned on {Context.Guild.Name} by {Context.Message.Author.Username} for {reason}.");
        }
        catch (Exception)
        {
            Log.Warn(nameof(Warn), $"Failed to message {user.Username}.");
        }

        DatabaseHandler.AddEntry(user.Id, reason, DatabaseType.Warn, Context.Message.Author.Id);
        await Logging.SendLogMessage("Warning issued",
            $"{Context.Message.Author.Username} warned {user.Username} for {reason}.", Color.Red);
        await ReplyAsync($"User warned for {reason}.");
    }
}