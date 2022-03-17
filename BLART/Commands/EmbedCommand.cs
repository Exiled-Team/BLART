namespace BLART.Commands;

using System.Text.RegularExpressions;
using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;

public class EmbedCommand : ModuleBase<SocketCommandContext>
{
    [Command("embed")]
    [Summary("Sends an embeded message into the specified channel.")]
    public async Task Embed(
        [Summary("The channel to send the embed to.")] ITextChannel channel,
        [Summary("The color to use.")] string color,
        [Summary("The title and contents to use.")] [Remainder] string contents)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.Message.Author))
        {
            await ReplyAsync(ErrorHandlingService.GetErrorMessage(ErrorCodes.PermissionDenied));
            return;
        }
        
        color = color.Replace("#", string.Empty);
        string title = string.Empty;
        Color c;
        
        foreach (Match match in Regex.Matches(contents, "\"([^\"]*)\""))
            title = match.ToString();

        contents = contents.Replace(title, string.Empty);
        
        if (string.IsNullOrEmpty(title))
        {
            await ReplyAsync("Unable to parse title from contents. Did you forget to use quotes?");
            return;
        }
        
        try
        {
            c = new Color((uint)ColorParsing.ToColorValue(color));
        }
        catch (Exception e)
        {
            Log.Error(nameof(Embed), e.Message);
            await ReplyAsync(
                $"Unable to parse {color} into a valid color. Please use HTML color codes.");
            return;
        }

        bool isSilent = contents.EndsWith("-silent");
        contents = contents.Replace("-silent", string.Empty);
        
        Log.Debug(nameof(Embed), title);
        Log.Debug(nameof(Embed), c);
        Log.Debug(nameof(Embed), contents);

        await channel.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed(title.Trim('"'), contents, c));
        if (!isSilent)
            await ReplyAsync("Done.");
        else
            await Context.Message.DeleteAsync();
    }
}