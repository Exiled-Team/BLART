namespace BLART.Modules;

using BLART.Services;
using Discord;
using Discord.WebSocket;

public class Logging
{
    private static SocketTextChannel? logChannel;

    public static SocketTextChannel LogChannel =>
        logChannel ??= (SocketTextChannel)Bot.Instance.Guild.GetChannel(Program.Config.LogsId);
    
    public static async Task OnMessageUpdated(
        Cacheable<IMessage, ulong> before,
        SocketMessage after,
        ISocketMessageChannel channel)
    {
        if ((await before.GetOrDownloadAsync()).Content != after.Content)
            await SendLogMessage("Message Edited",
                $"Author: {(await before.GetOrDownloadAsync()).Author.Mention}\nOriginal: {(await before.GetOrDownloadAsync()).Content}\nEdited: {after.Content}\nChannel: {channel.Name}",
                Color.Orange);
    }

    public static async Task OnMessageDeleted(
        Cacheable<IMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel) =>
        await SendLogMessage("Message Deleted", $"Author: {(await message.GetOrDownloadAsync()).Author.Mention}\nMessage: {(await message.GetOrDownloadAsync()).Content}\nChannel: {(await channel.GetOrDownloadAsync()).Name}",
            Color.DarkOrange);

    public static async Task OnUserJoined(SocketGuildUser arg) =>
        await SendLogMessage("User Joined", $"User {arg.Username} has joined the server.", Color.Gold);

    public static async Task OnUserBanned(SocketUser arg1, SocketGuild arg2) =>
        await SendLogMessage("User Banned", $"User {arg1.Username} was banned from the server.", Color.DarkRed);

    public static async Task OnUsedLeft(SocketGuild arg1, SocketUser arg2) =>
        await SendLogMessage("User Left", $"User {arg2.Username} left the server.", Color.Red);

    internal static async Task SendLogMessage(string title, string description, Color color) => await LogChannel.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed(title, description, color));
}