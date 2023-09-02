namespace BLART.Services;

using System.Reflection;
using Discord;

public class EmbedBuilderService
{
    public static string Footer => $"Benevolent Lawmaking, Analysis and Reinforcement Technology | {Assembly.GetExecutingAssembly().GetName().Version} | - Joker119";

    public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color)
    {
        Log.Info(nameof(CreateBasicEmbed), $"Sending embed {title}.");
        return await Task.Run(() => new EmbedBuilder().WithTitle(title).WithDescription(description).WithColor(color).WithCurrentTimestamp().WithFooter(Footer).Build());
    }

    public static async Task<Embed> CreateStickyMessage(string message, IGuildUser staff)
    {
        Log.Info(nameof(CreateBasicEmbed), $"Sending sticky embed {message}.");
        return await Task.Run(() => new EmbedBuilder().WithTitle("Stickied Message").WithDescription(message).WithColor(Color.Blue).WithCurrentTimestamp().WithFooter(Footer).WithAuthor(staff).Build());
    }
}