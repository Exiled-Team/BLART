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
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            return;
        }

        try
        {
            color = color.Replace("#", string.Empty);
        }
        catch (Exception)
        {
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.FailedToParseColor, color));
            return;
        }

        string title = string.Empty;
        Color c;
        
        foreach (Match match in Regex.Matches(contents, "\"([^\"]*)\""))
            title = match.ToString();

        if (string.IsNullOrEmpty(title))
        {
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.FailedToParseTitle));
            return;
        }

        contents = contents.Replace(title, string.Empty);
        
        try
        {
            c = new Color((uint)ColorParsing.ToColorValue(color));
        }
        catch (Exception e)
        {
            Log.Error(nameof(Embed), e.Message);
            await ReplyAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.FailedToParseColor, color));
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