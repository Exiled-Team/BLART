namespace BLART.Commands.Warns;

using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class WarningCommands
{
    [SlashCommand("info", "Gives information about a users warning(s), if any.")]
    public async Task WarnInfo([Discord.Commands.Summary("The user to get info for.")] SocketUser user)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        List<PunishmentInfo> infos = DatabaseHandler.GetPunishmentInfo(user.Id, DatabaseType.Warn);
        if (infos.Count <= 0)
        {
            await RespondAsync("There are no warnings for this user.");
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

        await RespondAsync(embed: builder.Build());
    }
}