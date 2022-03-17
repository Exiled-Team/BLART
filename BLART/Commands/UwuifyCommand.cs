namespace BLART.Commands;

using BLART.Modules;
using Discord.Commands;

public class UwuifyCommand : ModuleBase<SocketCommandContext>
{
    [Command("uwuify")]
    [Summary("Don't do it. Just don't.")]
    public async Task Uwuify([Summary("The message to uwuify")] [Remainder] string message) => 
        await ReplyAsync(await CatgirlShit.Uwu(message));
}