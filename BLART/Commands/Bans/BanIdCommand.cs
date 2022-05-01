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
    public async Task Ban([Summary("UserID", "The user to ban.")] string id, [Summary("Reason", "The reason for the ban.")][Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User, true))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        if (!ulong.TryParse(id, out ulong userId))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId, id),
                ephemeral: true);
            return;
        }

        reason = ReasonParsing.ParseRules(reason);
        await Context.Guild.AddBanAsync(userId, 7, reason);
        await Logging.SendLogMessage("User banned",
            $"{Context.User.Username} has banned {id} for {reason}.", Color.Red);
        DatabaseHandler.AddEntry(userId, reason, DatabaseType.Ban, Context.User.Id);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("User banned", $"{id} has been banned for: {reason}", Color.Orange));
    }
}