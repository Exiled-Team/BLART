namespace BLART.Commands.Bans;

using BLART.Modules;
using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class BanCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("unban", "Unbans the given user ID.")]
    public async Task Unban([Summary("The user ID to unban")] ulong id)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        await Context.Guild.RemoveBanAsync(id);
        PunishmentInfo? info = DatabaseHandler.GetInfoById(id, DatabaseType.Ban);
        DatabaseHandler.RemoveEntry(id, DatabaseType.Ban);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("User banned", $"{id} has been unbanned", Color.Green));
        await Logging.SendLogMessage("Ban removed",
            $"{Context.User.Username} removed ban for user {id} \n" +
            $"{(info != null ? $"Banned user: {Context.Guild.GetUsername(info.UserId)}\nIssued by: {Context.Guild.GetUsername(info.StaffId)} \nReason: {info.Reason}\nIssued on: {info.Issued}" : "Info unavailable.")}",
            Color.Gold);
    }
}