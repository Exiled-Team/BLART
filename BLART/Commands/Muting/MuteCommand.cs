namespace BLART.Commands.Muting;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("mute", "Commands for managing server mutes.")]
public partial class MuteCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add", "Mutes the indicated user for the given time period.")]
    public async Task Mute(
        [Summary("User", "The user to mute.")] SocketUser user,
        [Summary("Duration", "The time duration")] string duration,
        [Summary("Reason", "The reason for the mute.")] [Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }
        
        TimeSpan span = TimeParsing.ParseDuration(duration);
        if (span.Ticks <= 0)
        {
            Log.Error(nameof(Mute), $"{duration} failed to parse.");
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseDuration, duration));
            return;
        }

        if (user == Context.User)
        {
            await RespondAsync("No, don't do it, you have too much to live for!");
            return;
        }
        
        await Logging.SendLogMessage("User muted", $"{Context.User.Username} muted {user.Username} for {span} for {reason}.", Color.Orange);
        await ((IGuildUser)user).SetTimeOutAsync(span);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("User Muted",
            $"{user.Username} has been muted for {span}.\nReason: {reason}", Color.Red));
    }
}