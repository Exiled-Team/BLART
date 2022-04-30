namespace BLART.Commands.PingTriggers;

using System.Threading.Tasks;
using BLART.Services;
using BLART;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class TriggerCommands
{
    [SlashCommand("remove", "Removes your current ping trigger.")]
    public async Task DoRemoveTrigger()
    {
        DatabaseHandler.RemoveEntry(Context.User.Id, DatabaseType.Ping);
        await RespondAsync("Ping trigger removed.");
    }
}