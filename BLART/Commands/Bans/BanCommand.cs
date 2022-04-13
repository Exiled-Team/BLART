namespace BLART.Commands.Bans;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class BanCommand : ModuleBase<SocketCommandContext>
{
    [Command("ban")]
    [Summary("Bans the given user")]
    public async Task Ban([Summary("The user to ban.")] SocketUser user, [Summary("The reason for the ban.")][Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        reason = ReasonParsing.ParseRules(reason);
        try
        {
            await user.SendMessageAsync(
                $"You have been banned from {Context.Guild.Name} by {Context.Message.Author.Username} for {reason}");
        }
        catch (Exception)
        {
            Log.Warn(nameof(Ban), $"Failed to message {user.Username}");
        }

        await ((IGuildUser)user).BanAsync(7, reason);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
        await Logging.SendLogMessage("User banned",
            $"{Context.Message.Author.Username} has banned {user.Username} for {reason}.", Color.Red);
        DatabaseHandler.AddEntry(user.Id, reason, DatabaseType.Ban, Context.Message.Author.Id);
    }
}