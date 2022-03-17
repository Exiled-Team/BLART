namespace BLART.Commands;

using BLART.Services;
using Discord.Commands;

public class PrintCommand : ModuleBase<SocketCommandContext>
{
    [Command("print")]
    [Summary("DNI")]
    public Task Print([Remainder] string message)
    {
        Log.Info(nameof(Print), message);
        return Task.CompletedTask;
    }
}