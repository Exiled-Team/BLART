namespace BLART.Commands;

using BLART.Services;
using Discord;
using Discord.Commands;

public class RollCommand : ModuleBase<SocketCommandContext>
{
    [Command("roll")]
    [Summary("Rolls dice.")]
    public async Task Roll([Summary("The number of sides on the dice")] int sides = 6)
    {
        int r = Program.Rng.Next(1, sides);
        await ReplyAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Dice roll", r.ToString(), Color.Magenta));
    }
}