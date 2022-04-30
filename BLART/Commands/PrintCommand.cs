namespace BLART.Commands;

using BLART.Services;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public class PrintCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("print", "DNI")]
    public Task Print([Remainder] string message)
    {
        Log.Info(nameof(Print), message);
        return Task.CompletedTask;
    }
}