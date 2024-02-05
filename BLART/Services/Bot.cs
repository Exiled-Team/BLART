using BLART.Commands;
using BLART.Modals;
using BLART.Modules;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace BLART.Services;

public class Bot : BackgroundService
{
    private DiscordSocketClient? _client;
    private SocketGuild? _guild;
    private readonly ILogger<Bot> _logger;
    private readonly Config _config;

    public Bot(ILogger<Bot> logger, Config config)
    {
        _logger = logger;
        _config = config;
    }

    public SocketGuild Guild => _guild ??= Client.Guilds.FirstOrDefault(g => g.Id == 656673194693885975)!;
    public string ReplyEmote => "<:yesexiled:813850607294218251>";
    private DiscordSocketClient Client => _client ??= new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.All, AlwaysDownloadUsers = true, MessageCacheSize = 10000, });
    private InteractionService InteractionService { get; set; } = null!;
    private SlashCommandHandler SlashCommandHandler { get; set; } = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, _config.BotToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Token validation failed");
            return;
        }

        InteractionService = new InteractionService(Client);
        SlashCommandHandler = new SlashCommandHandler(InteractionService, Client);
        
        InteractionService.Log += Log;
        Client.Log += Log;
        Client.MessageDeleted += Logging.OnMessageDeleted;
        Client.MessageUpdated += Logging.OnMessageUpdated;
        Client.UserJoined += Logging.OnUserJoined;
        Client.UserBanned += Logging.OnUserBanned;
        Client.UserLeft += Logging.OnUsedLeft;
        
        
        Client.MessageReceived += PingTriggers.HandleMessage;
        Client.MessageReceived += SpamPrevention.OnMessageReceived;
        Client.MessageUpdated += SpamPrevention.OnMessageUpdated;
        
        
        Client.UserVoiceStateUpdated += ChannelRenting.OnVoiceStateChanged;

        
        Client.UserJoined += RaidProtection.OnUserJoined;

        
        await SlashCommandHandler.InstallCommandsAsync();

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
        
        await Client.LoginAsync(TokenType.Bot, _config.BotToken);
        await Client.StartAsync();

        await Task.Delay(-1, stoppingToken);
    }

    private Task Log(LogMessage arg)
    {
        switch (arg.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                _logger.LogError(arg.Exception, "{Message}", arg.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("{Message}", arg.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{Message}", arg.Message);
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                _logger.LogDebug("{Message}", arg.Message);
                break;
        }

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.LogoutAsync();
        await base.StopAsync(cancellationToken);
    }
}