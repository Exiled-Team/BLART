namespace BLART.Commands.Muting;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class MuteCommand : ModuleBase<SocketCommandContext>
{
    [Command("mute")]
    [Summary("Mutes the indicated user for the given time period.")]
    public async Task Mute(
        [Summary("The user to mute.")] SocketUser user,
        [Summary("The time duration")] string duration,
        [Summary("The reason for the mute.")] [Remainder] string reason)
    {
        Log.Debug(nameof(Mute), "Muting.");
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            Log.Debug(nameof(Mute), "Can't run staff commands.");
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        Log.Debug(nameof(Mute), "Can run commands");
        TimeSpan span = TimeParsing.ParseDuration(duration);
        Log.Debug(nameof(Mute), span);
        if (span.Ticks <= 0)
        {
            Log.Error(nameof(Mute), $"{duration} failed to parse.");
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.UnableToParseDuration));
            return;
        }

        Log.Debug(nameof(Mute), "stuff should happen.");
        await Logging.SendLogMessage("User muted", $"{Context.Message.Author.Username} muted {user.Username} for {span} for {reason}.", Color.Orange);
        await ((IGuildUser)user).SetTimeOutAsync(span);
        await ReplyAsync($"User muted for {span}. Reason: {reason}.");
        Log.Debug(nameof(Mute), "stuff happened.");
    }
}