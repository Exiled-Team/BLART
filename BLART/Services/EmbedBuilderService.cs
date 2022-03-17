namespace BLART.Services;

using Discord;

public class EmbedBuilderService
{
    public static string Footer => "Benevolent Lawmaking, Analysis and Reinforcement Technology - Joker119";
    
    public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color) => await Task.Run(
        () => new EmbedBuilder().WithTitle(title).WithDescription(description).WithColor(color).WithCurrentTimestamp()
            .WithFooter(Footer).Build());

    public static async Task<Embed> CreateErrorEmbed(string source, string error) => await Task.Run(() =>
        new EmbedBuilder().WithTitle($"An error occured from {source}").WithDescription($"**Error Details**:\n{error}")
            .WithColor(Color.DarkRed).WithCurrentTimestamp().WithFooter(Footer).Build());
}