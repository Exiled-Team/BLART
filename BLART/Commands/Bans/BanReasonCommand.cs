namespace BLART.Commands.Bans;

using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Rest;

public class BanReasonCommand : ModuleBase<SocketCommandContext>
{
    [Command("baninfo")]
    [Summary("Gets information (if available) about a user's ban.")]
    public async Task BanInfo([Summary("The user ID to get info for.")] ulong userId)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        PunishmentInfo? info = DatabaseHandler.GetInfoById(userId, DatabaseType.Ban);
        if (info == null)
        {
            RestBan ban = await Context.Guild.GetBanAsync(userId);
            await ReplyAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Ban Information",
                $"The given user was banned for {ban.Reason}", Color.Orange));
            return;
        }

        await ReplyAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Ban Information",
            $"Issued on: {info.Issued}\nIssued by: {Context.Guild.GetUsername(info.StaffId)}\nReason: {info.Reason}",
            Color.Orange));
    }
}