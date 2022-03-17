namespace BLART.Commands;

using System.Text;
using BLART.Services;
using Discord;
using Discord.Commands;

public class HelpCommand : ModuleBase<SocketCommandContext>
{
    [Command("help")]
    [Summary("Lists all available commands.")]
    public async Task Help()
    {
        EmbedBuilder builder = new();
        builder.WithColor(Color.Purple);
        builder.WithCurrentTimestamp();
        builder.WithTitle("Commands");
        builder.WithFooter(EmbedBuilderService.Footer);

        int count = 0;
        foreach (CommandInfo command in Bot.Instance.CommandService.Commands)
        {
            if (command.Summary == "DNI")
                continue;
            count++;
            List<string> args = new();
            foreach (ParameterInfo arg in command.Parameters)
                args.Add($"{arg.Name} -- {arg.Summary}");

            StringBuilder stringBuilder = new();
            foreach (string s in args)
                if (!string.IsNullOrEmpty(s))
                    stringBuilder.AppendLine(s);
            stringBuilder.AppendLine(command.Summary ?? "No description");
            builder.AddField(command.Name, stringBuilder.ToString());

            if (count == 24)
            {
                await ReplyAsync(embed: builder.Build());
                builder = new();
                builder.WithColor(Color.Purple);
                builder.WithCurrentTimestamp();
                builder.WithTitle("Commands Contd.");
                builder.WithFooter(EmbedBuilderService.Footer);
                count = 0;
            }
        }

        await ReplyAsync(embed: builder.Build());
    }
}