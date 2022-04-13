namespace BLART.Commands.Bans;

using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;

public class BanIdCommand : ModuleBase<SocketCommandContext>
{
    [Command("banid")]
    [Summary("Bans the given user ID")]
    public async Task Ban([Summary("The user to ban.")] ulong user, [Summary("The reason for the ban.")][Remainder] string reason)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }

        reason = ReasonParsing.ParseRules(reason);
        await Context.Guild.AddBanAsync(user, 7, reason);
        await Logging.SendLogMessage("User banned",
            $"{Context.Message.Author.Username} has banned {user} for {reason}.", Color.Red);
        DatabaseHandler.AddEntry(user, reason, DatabaseType.Ban, Context.Message.Author.Id);
        await Context.Message.AddReactionAsync(Emote.Parse(Bot.Instance.ReplyEmote));
    }
}