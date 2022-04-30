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
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }
        
        TimeSpan span = TimeParsing.ParseDuration(duration);
        if (span.Ticks <= 0)
        {
            Log.Error(nameof(Mute), $"{duration} failed to parse.");
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseDuration, duration));
            return;
        }

        if (user == Context.Message.Author)
        {
            await ReplyAsync("No, don't do it, you have to much to live for!");
            return;
        }
        
        await Logging.SendLogMessage("User muted", $"{Context.Message.Author.Username} muted {user.Username} for {span} for {reason}.", Color.Orange);
        await ((IGuildUser)user).SetTimeOutAsync(span);
        await ReplyAsync($"User muted for {span}. Reason: {reason}.");
    }
}