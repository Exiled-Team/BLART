namespace BLART.Commands;

using BLART.Services;
using Discord;
using Discord.Commands;

public class PebkacCommand : ModuleBase<SocketCommandContext>
{
    [Command("pebkac")]
    [Summary("Tells a user they are being a 4-head.")]
    public async Task Pebkac()
    {
        IGuildUser user = (IGuildUser)Context.Message.Author;
        if (user.RoleIds.All(r => r != 656673336402640902 && r != 668651927298375690 && r != 656673780332101648))
        {
            await Context.Channel.SendMessageAsync("YOU'RE a PEBKAC.");
            return;
        }
        
        string message =
            "Hello! I'm here to tell you that the issue you have reported is not, infact, a bug/issue within the code of whatever you reported this for.\n\n" +
            "This is infact a user-error, otherwise known as P.E.B.K.A.C. (Problem Exists Between Keyboard And Chair)\n\n" +
            "Here are some friendly tips:\n" +
            "1. Try reading the error message. If this is config related, the first line of the error will say '(Line:' followed by a number. This number is the line number of the config file that broke everything.\n" +
            "2. Ensure that your plugins are all properly updated. When reporting bugs, make sure you include the version you are using. ('latest', 'newest', etc. are NOT versions)\n" +
            "3. Make sure you have read the FAQ.\n" +
            "4. Make sure you are reading the ENTIRE console, including the quickly scrolled through text during server startup, and look for any red text.\n" +
            "5. Follow directions for how to setup/configure a given plugin or EXILED itself.\n" +
            "6. Read all of the descriptions of all the config values before you ask any config-related questions. Chances are your question has already been answered.";

        await Context.Channel.SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("P.E.B.K.A.C.", message, Color.Gold));
    }
}