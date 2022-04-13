namespace BLART.Commands.Bans;

using BLART.Modules;
using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;

public class UnbanIdCommand : ModuleBase<SocketCommandContext>
{
    [Command("unban")]
    [Summary("Unbans the given user ID.")]
    public async Task Unban([Summary("The user ID to unban")] ulong id)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        await Context.Guild.RemoveBanAsync(id);
        PunishmentInfo? info = DatabaseHandler.GetInfoById(id, DatabaseType.Ban);
        DatabaseHandler.RemoveEntry(id, DatabaseType.Ban);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
        await Logging.SendLogMessage("Ban removed",
            $"{Context.Message.Author.Username} removed ban for user {id} \n" +
            $"{(info != null ? $"Banned user: {Context.Guild.GetUsername(info.UserId)}\nIssued by: {Context.Guild.GetUsername(info.StaffId)} \nReason: {info.Reason}\nIssued on: {info.Issued}" : "Info unavailable.")}",
            Color.Gold);
    }
}