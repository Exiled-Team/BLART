namespace BLART.Commands.Bans;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class BanCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("id", "Bans the given user ID.")]
    public async Task Ban([Summary("UserID", "The user to ban.")] ulong user, [Summary("Reason", "The reason for the ban.")][Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        reason = ReasonParsing.ParseRules(reason);
        await Context.Guild.AddBanAsync(user, 7, reason);
        await Logging.SendLogMessage("User banned",
            $"{Context.User.Username} has banned {user} for {reason}.", Color.Red);
        DatabaseHandler.AddEntry(user, reason, DatabaseType.Ban, Context.User.Id);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("User banned", $"{user} has been banned for: {reason}", Color.Orange));
    }
}