namespace BLART;

using BLART.Commands;
using BLART.Modules;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using BLART.SlashCommands;
using Discord.Interactions;

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

    ~Bot()
    {
        Client.StopAsync();
        Client.LogoutAsync();
    }

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
        
        Log.Debug(nameof(Init), "Initializing Database..");
        DatabaseHandler.Init(args.Contains("--updatetables"));
        
        Log.Debug(nameof(Init), "Initializing Text Commands..");
        CommandService = new CommandService();
        CommandHandler = new CommandHandler(Client, CommandService);
        
        Log.Debug(nameof(Init), "Initializing Slash Commands..");
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
        Client.Ready += async () => //just for a showcase, move this if you feel like it
        {
            int slashCommandsRegistered = (await InteractionService.RegisterCommandsGloballyAsync(true)).Count;

            //for joker, making changes to the user visible part of global slash commands, Eg. Removing, changing arguments, removing the command will take up to an hour as it is cached.
            //If you want to, you could subscribe to the ready event an loop through all guilds and register the commands to each guild manually, or hardcode the exiled serverid
            //Guild command user visible changes are instant. using the line below, ofc it should be put into a foreach and called with the guildid, or the serverid put in manually
            //int slashCommandsRegistered = (await InteractionService.RegisterCommandsToGuildAsync(guildidhere, true)).Count;
            Log.Debug(nameof(Init), $"Registered {slashCommandsRegistered} interaction module");
        };

        Log.Debug(nameof(Init), "Logging in..");
        await Client.LoginAsync(TokenType.Bot, Program.Config.BotToken);
        await Client.StartAsync();

        await Task.Delay(-1);
    }
}