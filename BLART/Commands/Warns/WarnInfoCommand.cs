namespace BLART.Commands.Warns;

using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class WarnInfoCommand : ModuleBase<SocketCommandContext>
{
    [Command("warninfo")]
    [Summary("Gives information about a users warning(s), if any.")]
    public async Task WarnInfo([Summary("The user to get info for.")] SocketUser user)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        List<PunishmentInfo> infos = DatabaseHandler.GetPunishmentInfo(user.Id, DatabaseType.Warn);
        if (infos.Count <= 0)
        {
            await ReplyAsync("There are no warnings for this user.");
            return;
        }

        EmbedBuilder builder = new();
        builder.WithTitle($"Warning Information for {user.Username}");
        builder.WithCurrentTimestamp();
        builder.WithColor(Color.Orange);
        builder.WithFooter(EmbedBuilderService.Footer);

        foreach (PunishmentInfo info in infos)
        {
            builder.AddField($"Warning ID: {info.Id}",
                $"Issued by: {Context.Guild.GetUsername(info.StaffId)}\nIssued on: {info.Issued}\nReason: {info.Reason}");
        }

        await ReplyAsync(embed: builder.Build());
    }
}