namespace BLART.Commands.Muting;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class MuteCommands
{
    [SlashCommand("unmute", "Unmutes the specified user.")]
    public async Task Unmute([Summary("The user to unmute")] SocketUser user)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        await Logging.SendLogMessage("User unmuted", $"{Context.User.Username} unmuted {user.Username}.", Color.Orange);
        await ((IGuildUser)user).RemoveTimeOutAsync();
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Mute Removed",
            $"{user.Username} has had their mute removed.", Color.Green));
    }
}