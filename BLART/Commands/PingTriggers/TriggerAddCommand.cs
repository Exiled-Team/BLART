namespace BLART.Commands.PingTriggers;

using System.Threading.Tasks;
using BLART.Services;
using Discord.Commands;

public class TriggerAddCommand : ModuleBase<SocketCommandContext>
{
    [Command("pt add")]
    [Summary("Adds a ping trigger.")]
    public async Task AddPingTrigger([Summary("The message to be sent.")] [Remainder] string message)
    {
        if (message.Length > Program.Config.TriggerLengthLimit)
        {
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.TriggerLengthExceedsLimit, Program.Config.TriggerLengthLimit.ToString()));
            return;
        }

        bool flag = false;
        if (!string.IsNullOrEmpty(DatabaseHandler.GetPingTrigger(Context.Message.Author.Id)))
        {
            DatabaseHandler.RemoveEntry(Context.Message.Author.Id, DatabaseType.Ping);
            flag = true;
        }

        DatabaseHandler.AddEntry(Context.Message.Author.Id, message, DatabaseType.Ping);
        await ReplyAsync($"Ping trigger {(flag ? "changed" : "added")}.");
    }
}