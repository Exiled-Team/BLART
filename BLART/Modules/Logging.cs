namespace BLART.Modules;

using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using IResult = Discord.Commands.IResult;

public class Logging
{
    private static SocketTextChannel? logChannel;

    public static SocketTextChannel LogChannel =>
        logChannel ??= (SocketTextChannel)Bot.Instance.Guild.GetChannel(Program.Config.LogsId);
    
    public static async Task OnMessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        if ((await before.GetOrDownloadAsync()).Content != after.Content)
            await SendLogMessage("Message Edited",
                $"Author: {(await before.GetOrDownloadAsync()).Author.Mention}\nOriginal: {(await before.GetOrDownloadAsync()).Content}\nEdited: {after.Content}\nChannel: <#{channel.Id}>",
                Color.Orange);
    }

    public static async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel) =>
        await SendLogMessage("Message Deleted", $"Author: {(await message.GetOrDownloadAsync()).Author.Mention}\nMessage: {(await message.GetOrDownloadAsync()).Content}\nChannel: <#{(await channel.GetOrDownloadAsync()).Id}>", Color.DarkOrange);

    public static async Task OnUserJoined(SocketGuildUser arg) =>
        await SendLogMessage("User Joined", $"User {arg.Username} has joined the server.", Color.Gold);

    public static async Task OnUserBanned(SocketUser arg1, SocketGuild arg2) =>
        await SendLogMessage("User Banned", $"User {arg1.Username} was banned from the server.", Color.DarkRed);

    public static async Task OnUsedLeft(SocketGuild arg1, SocketUser arg2) =>
        await SendLogMessage("User Left", $"User {arg2.Username} left the server.", Color.Red);

    internal static async Task SendLogMessage(string title, string description, Color color) => await LogChannel.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed(title, description, color));

    public static Task ModalLogging(SocketModal arg)
    {
        if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "Modals.log")))
            File.Create(Path.Combine(Environment.CurrentDirectory, "Modals.log")).Close();

        _ = File.AppendAllTextAsync(Path.Combine(Environment.CurrentDirectory, "Modal.log"), $"[{DateTime.Now}]: {arg.User.Username} submitted modal {arg.Data.CustomId}");
        return Task.CompletedTask;
    }

    public static Task ButtonLogging(SocketMessageComponent arg)
    {
        if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "Buttons.log")))
            File.Create(Path.Combine(Environment.CurrentDirectory, "Buttons.log")).Close();

        _ = File.AppendAllTextAsync(Path.Combine(Environment.CurrentDirectory, "Buttons.log"),
            $"[{DateTime.Now}]: {arg.User.Username} submitted button {arg.Data.CustomId}");
        return Task.CompletedTask;
    }

    public static Task CommandLogging(ICommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
    {
        if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "Commands.log")))
            File.Create(Path.Combine(Environment.CurrentDirectory, "Commands.log")).Close();
        
        _ = File.AppendAllTextAsync(Path.Combine(Environment.CurrentDirectory, "Commands.log"),
            $"[{DateTime.Now}]: {arg2.User.Username} submitted command {arg1.Name} ()");
        return Task.CompletedTask;
    }
}