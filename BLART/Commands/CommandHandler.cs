namespace BLART.Commands;

using System.Reflection;
using BLART.TypeReaders;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Services;

public class CommandHandler
{
    private readonly DiscordSocketClient client;
    private readonly CommandService service;

    public CommandHandler(DiscordSocketClient client, CommandService service)
    {
        this.client = client;
        this.service = service;
    }

    public async Task InstallCommandsAsync()
    {
        client.MessageReceived += HandleCommandAsync;
        service.AddTypeReader(typeof(IEmote), new EmoteTypeReader());
        await service.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
    }

    private async Task HandleCommandAsync(SocketMessage message)
    {
        if (message is not SocketUserMessage msg)
            return;

        int argPos = 0;
        if (!(msg.HasStringPrefix(Program.Config.BotPrefix, ref argPos) ||
              msg.HasMentionPrefix(client.CurrentUser, ref argPos)) || msg.Author.IsBot)
            return;

        SocketCommandContext context = new(client, msg);

        try
        {
            await service.ExecuteAsync(context, argPos, null);
        }
        catch (Exception e)
        {
            Log.Error(nameof(HandleCommandAsync), $"Error executing command: {message.Content}\n{e}");
            await message.Channel.SendMessageAsync(
                embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, e.Message));
        }
    }

    public static bool CanRunStaffCmd(SocketUser user, bool strict) => CanRunStaffCmd((IGuildUser)user, strict);

    public static bool CanRunStaffCmd(IGuildUser user, bool strict) => user.RoleIds.Any(roleId => roleId == Program.Config.DiscStaffId || roleId == Program.Config.ContributorId && !strict) || user.GuildPermissions.Administrator || user.GuildPermissions.ManageChannels;
}