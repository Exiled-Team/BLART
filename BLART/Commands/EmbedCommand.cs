namespace BLART.Commands;

using System.Text.RegularExpressions;
using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("embed", "Commands for managing embeded messages.")]
public partial class EmbedCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("send", "Sends an embeded message into a specific channel.")]
    public async Task Embed(
        [Summary("Channel", "The channel to send the embed to.")] ITextChannel channel,
        [Summary("Color", "The color to use.")] string color,
        [Summary("Contents", "The title and contents to use.")] [Remainder] string contents)
    {
        if (!CommandHandler.CanRunStaffCmd(Context.User))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        try
        {
            color = color.Replace("#", string.Empty);
        }
        catch (Exception)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.FailedToParseColor, color));
            return;
        }

        string title = string.Empty;
        Color c;
        
        foreach (Match match in Regex.Matches(contents, "\"([^\"]*)\""))
            title = match.ToString();

        if (string.IsNullOrEmpty(title))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.FailedToParseTitle));
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
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.FailedToParseColor, color));
            return;
        }

        bool isSilent = contents.EndsWith("-silent");
        contents = contents.Replace("-silent", string.Empty);
        
        Log.Debug(nameof(Embed), title);
        Log.Debug(nameof(Embed), c);
        Log.Debug(nameof(Embed), contents);

        await channel.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed(title.Trim('"'), contents, c));
        if (!isSilent)
            await RespondAsync("Done.");
    }
}