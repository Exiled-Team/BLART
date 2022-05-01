namespace BLART.Commands.Warns;

using BLART.Modules;
using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("warn", "Commands for managing warnings.")]
public partial class WarningCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("remove", "Removes the indicated warning.")]
    public async Task Unwarn([Summary("WarningID", "The ID number of the warning to remove.")] int id)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        PunishmentInfo? info = DatabaseHandler.GetInfoById(id, DatabaseType.Warn);
        DatabaseHandler.RemoveEntry(id, DatabaseType.Warn);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Warning removed",
            $"Warning {id} has been removed.", Color.Red));
        await Logging.SendLogMessage("Warning removed",
            $"{Context.User.Username} removed warning {id} \n" +
            $"{(info != null ? $"Warned user: {Context.Guild.GetUsername(info.UserId)}\nIssued by: {Context.Guild.GetUsername(info.StaffId)} \nReason: {info.Reason}\nIssued on: {info.Issued}" : "Info unavailable.")}",
            Color.Gold);
    }
}