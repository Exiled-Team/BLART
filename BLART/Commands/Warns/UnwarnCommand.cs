namespace BLART.Commands.Warns;

using BLART.Modules;
using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;

public class UnwarnCommand : ModuleBase<SocketCommandContext>
{
    [Command("unwarn")]
    [Summary("Removes the indicated warning.")]
    public async Task Unwarn([Summary("The ID number of the warning to remove.")] int id)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        PunishmentInfo? info = DatabaseHandler.GetInfoById(id, DatabaseType.Warn);
        DatabaseHandler.RemoveEntry(id, DatabaseType.Warn);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
        await Logging.SendLogMessage("Warning removed",
            $"{Context.Message.Author.Username} removed warning {id} \n" +
            $"{(info != null ? $"Warned user: {Context.Guild.GetUsername(info.UserId)}\nIssued by: {Context.Guild.GetUsername(info.StaffId)} \nReason: {info.Reason}\nIssued on: {info.Issued}" : "Info unavailable.")}",
            Color.Gold);
    }
}