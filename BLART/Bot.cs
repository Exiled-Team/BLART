namespace BLART;

using BLART.Commands;
using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Modals;

public class Bot
{
    private DiscordSocketClient? client;
    private SocketGuild? guild;

    public static Bot Instance { get; private set; } = null!;

    public SocketGuild Guild => guild ??= Client.Guilds.FirstOrDefault(g => g.Id == 656673194693885975)!;
    public string ReplyEmote => "<:yesexiled:813850607294218251>";
    private DiscordSocketClient Client => client ??= new DiscordSocketClient(new DiscordSocketConfig { AlwaysDownloadUsers = true, MessageCacheSize = 10000, });
    public CommandService CommandService { get; private set; } = null!;
    public CommandHandler CommandHandler { get; private set; } = null!;
    public InteractionService InteractionService { get; private set; } = null!;
    public SlashCommandHandler SlashCommandHandler { get; private set; } = null!;

    public Bot(string[] args)
    {
        Instance = this;
        Init(args).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Terminates the bot.
    /// </summary>
    public void Destroy() => Client.LogoutAsync();

    private async Task Init(string[] args)
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

        Log.Debug(nameof(Init), "Initializing Text Commands..");
        CommandService = new CommandService();
        CommandHandler = new CommandHandler(Client, CommandService);

        Log.Debug(nameof(Init), "Initializing Slash commands..");
        InteractionService = new InteractionService(Client);
        SlashCommandHandler = new SlashCommandHandler(InteractionService, Client);

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
        Client.MessageUpdated += SpamPrevention.OnMessageUpdated;
        
        Log.Debug(nameof(Init), "Setting up channel renting..");
        Client.UserVoiceStateUpdated += ChannelRenting.OnVoiceStateChanged;

        Log.Debug(nameof(Init), "Setting up raid protection..");
        Client.UserJoined += RaidProtection.OnUserJoined;
        
        Log.Debug(nameof(Init), "Installing text commands..");
        await CommandHandler.InstallCommandsAsync();

        Log.Debug(nameof(Init), "Installing slash commands..");
        await SlashCommandHandler.InstallCommandsAsync();
        Client.Ready += async () =>
        {
            Log.Debug(nameof(Init), "Initializing Database..");
            await DatabaseHandler.Init(args.Contains("--updatetables"));

            int slashCommandsRegistered = (await InteractionService.RegisterCommandsToGuildAsync(Guild.Id)).Count;

            Log.Debug(nameof(Init), $"Registered {slashCommandsRegistered} interaction modules.");
        };

        Log.Debug(nameof(Init), "Registering Modal handlers..");
        Client.ModalSubmitted += BugReportModal.HandleModal;
        Client.ModalSubmitted += EmbedModal.HandleModal;
        Client.ButtonExecuted += BugReportModal.HandleButton;
        Client.ButtonExecuted += EmbedModal.HandleButton;
        
        Log.Debug(nameof(Init), "Logging in..");
        await Client.LoginAsync(TokenType.Bot, Program.Config.BotToken);
        await Client.StartAsync();

        await Task.Delay(-1);
    }
}