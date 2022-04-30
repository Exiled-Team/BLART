namespace BLART.Commands.Warns;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class WarningCommands
{
    [SlashCommand("add", "Warns the indicated user.")]
    public async Task Warn(
        [Discord.Commands.Summary("The user to warn")] SocketUser user,
        [Discord.Commands.Summary("The reason for the warning.")] [Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        reason = ReasonParsing.ParseRules(reason);
        try
        {
            await user.SendMessageAsync(
                $"You have been warned on {Context.Guild.Name} by {Context.User.Username} for {reason}.");
        }
        catch (Exception)
        {
            Log.Warn(nameof(Warn), $"Failed to message {user.Username}.");
        }

        DatabaseHandler.AddEntry(user.Id, reason, DatabaseType.Warn, Context.User.Id);
        await Logging.SendLogMessage("Warning issued",
            $"{Context.User.Username} warned {user.Username} for {reason}.", Color.Red);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("User warned",
            $"{user.Username} warned for {reason}", Color.Red));
    }
}