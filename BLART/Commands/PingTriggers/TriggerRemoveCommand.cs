namespace BLART.Commands.PingTriggers;

using System.Threading.Tasks;
using BLART.Services;
using BLART;
using Discord.Commands;

public class TriggerRemoveCommand : ModuleBase<SocketCommandContext>
{
    [Command("pt remove")]
    [Summary("Removes your current ping trigger.")]
    public async Task DoRemoveTrigger()
    {
        DatabaseHandler.RemoveEntry(Context.Message.Author.Id, DatabaseType.Ping);
        await ReplyAsync("Ping trigger removed.");
    }
}