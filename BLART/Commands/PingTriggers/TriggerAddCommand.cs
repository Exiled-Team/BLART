namespace BLART.Commands.PingTriggers;

using System.Threading.Tasks;
using BLART.Services;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("pt", "Commands for managing ping triggers.")]
public partial class TriggerCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add", "Adds a ping trigger.")]
    public async Task AddPingTrigger([Summary("Message", "The message to be sent.")] [Remainder] string message)
    {
        if (message.Length > Program.Config.TriggerLengthLimit)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.TriggerLengthExceedsLimit, Program.Config.TriggerLengthLimit.ToString()), ephemeral: true);
            return;
        }

        bool flag = false;
        if (!string.IsNullOrEmpty(DatabaseHandler.GetPingTrigger(Context.User.Id)))
        {
            DatabaseHandler.RemoveEntry(Context.User.Id, DatabaseType.Ping);
            flag = true;
        }

        DatabaseHandler.AddEntry(Context.User.Id, message, DatabaseType.Ping);
        await RespondAsync($"Ping trigger {(flag ? "changed" : "added")}.", ephemeral: true);
    }
}