namespace BLART;

using BLART.Commands;
using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class Bot
{
    private DiscordSocketClient? client;
    private SocketGuild? guild;

    public static Bot Instance { get; private set; } = null!;

    public SocketGuild Guild => guild ??= Client.Guilds.FirstOrDefault()!;
    private DiscordSocketClient Client => client ??= new DiscordSocketClient(new DiscordSocketConfig { AlwaysDownloadUsers = true, MessageCacheSize = 1000 });
    public CommandService CommandService { get; private set; } = null!;
    public CommandHandler CommandHandler { get; private set; } = null!;

    public Bot()
    {
        Instance = this;
        Init().GetAwaiter().GetResult();
    }

    ~Bot()
    {
        Client.StopAsync();
        Client.LogoutAsync();
    }

    private async Task Init()
    {
        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, Program.Config.BotToken);
        }
        catch (Exception e)
        {
            Log.Error(nameof(Init), e);
            return;
        }
        
        Log.Debug(nameof(Init), "Initializing Database..");
        DatabaseHandler.Init();
        
        Log.Debug(nameof(Init), "Initializing Commands..");
        CommandService = new CommandService();
        CommandHandler = new CommandHandler(Client, CommandService);

        Log.Debug(nameof(Init), "Setting up logging..");
        CommandService.Log += Log.Send;
        Client.Log += Log.Send;
        Client.MessageDeleted += Logging.OnMessageDeleted;
        Client.MessageUpdated += Logging.OnMessageUpdated;
        Client.UserJoined += Logging.OnUserJoined;
        Client.UserBanned += Logging.OnUserBanned;
        Client.UserLeft += Logging.OnUsedLeft;
        
        Log.Debug(nameof(Init), "Setting up message handlers..");
        Client.MessageReceived += PingTriggers.HandleMessage;
        Client.MessageReceived += SpamPrevention.OnMessageReceived;
        
        Log.Debug(nameof(Init), "Setting up raid protection..");
        Client.UserJoined += RaidProtection.OnUserJoined;
        
        Log.Debug(nameof(Init), "Installing commands..");
        await CommandHandler.InstallCommandsAsync();
        
        Log.Debug(nameof(Init), "Logging in..");
        await Client.LoginAsync(TokenType.Bot, Program.Config.BotToken);
        await Client.StartAsync();

        await Task.Delay(-1);
    }
}