namespace BLART.Commands.Bans;

using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Rest;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class BanCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("info", "Gets information (if available) about a user's ban.")]
    public async Task BanInfo([Summary("UserID", "The user ID to get info for.")] ulong userId)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        PunishmentInfo? info = DatabaseHandler.GetInfoById(userId, DatabaseType.Ban);
        if (info == null)
        {
            RestBan ban = await Context.Guild.GetBanAsync(userId);
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Ban Information",
                $"The given user was banned for {ban.Reason}", Color.Orange));
            return;
        }

        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Ban Information",
            $"Issued on: {info.Issued}\nIssued by: {Context.Guild.GetUsername(info.StaffId)}\nReason: {info.Reason}",
            Color.Orange));
    }
}