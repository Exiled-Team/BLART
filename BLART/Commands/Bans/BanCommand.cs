namespace BLART.Commands.Bans;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("ban", "Commands for banning users.")]
public partial class BanCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("user", "Bans the given user.")]
    public async Task Ban([Summary("User", "The user to ban.")] SocketUser user, [Summary("Reason", "The reason for the ban.")][Remainder] string reason)
    {
        if (((IGuildUser) user).RoleIds.Any(r => r == 656673336402640902) && ((IGuildUser)Context.User).RoleIds.All(r => r != 656673336402640902))
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Trololol",
                "<:O5Reverse:741817106718392412>", Color.Gold));
            await Task.Delay(1500);
            await ((IGuildUser) Context.User).BanAsync(0, "trolled");

            return;
        }

        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        reason = ReasonParsing.ParseRules(reason);
        try
        {
            await user.SendMessageAsync(
                $"You have been banned from {Context.Guild.Name} by {Context.User.Username} for {reason}");
        }
        catch (Exception)
        {
            Log.Warn(nameof(Ban), $"Failed to message {user.Username}");
        }
        
        await ((IGuildUser)user).BanAsync(7, reason);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("User banned", $"{user.Username} has been banned for: {reason}", Color.Orange));
        await Logging.SendLogMessage("User banned",
            $"{Context.User.Username} has banned {user.Username} for {reason}.", Color.Red);
        DatabaseHandler.AddEntry(user.Id, reason, DatabaseType.Ban, Context.User.Id);
    }
}