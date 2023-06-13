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
    private DiscordSocketClient Client => client ??= new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.All, AlwaysDownloadUsers = true, MessageCacheSize = 10000, });
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

        Log.Debug(nameof(Init), "Initializing Slash commands..");
        InteractionService = new InteractionService(Client);
        SlashCommandHandler = new SlashCommandHandler(InteractionService, Client);

        Log.Debug(nameof(Init), "Setting up logging..");
        InteractionService.Log += Log.Send;
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
        Client.ModalSubmitted += ReportUserModal.HandleModal;
        Client.ModalSubmitted += PluginSubmissionModal.HandleModal;
        Client.ModalSubmitted += TagModal.HandleModal;
        Client.ButtonExecuted += BugReportModal.HandleButton;
        Client.ButtonExecuted += EmbedModal.HandleButton;
        Client.ButtonExecuted += PluginSubmissionModal.HandleButton;
        Client.ButtonExecuted += SyncRolesModal.HandleButton;
        Client.ModalSubmitted += Logging.ModalLogging;
        Client.ButtonExecuted += Logging.ButtonLogging;
        InteractionService.InteractionExecuted += Logging.CommandLogging;
        
        Log.Debug(nameof(Init), "Logging in..");
        await Client.LoginAsync(TokenType.Bot, Program.Config.BotToken);
        await Client.StartAsync();

        _ = Task.Run(ServerCountUpdater.DoUpdate);

        await Task.Delay(-1);
    }
}