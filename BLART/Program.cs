using BLART;
using BLART.Services;
using Serilog;

var builder = WebApplication.CreateBuilder();


var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        logging.ClearProviders().AddSerilog(logger);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<Config>();
        services.AddHostedService<DbInitService>();
        services.AddSingleton<BotClientService>();
        services.AddHostedService<Bot>();
    })
    .Build();

/*public static class Program
{
    private static Bot? _bot;

    public static string DatabaseFile { get; } = Path.Combine(Environment.CurrentDirectory, "Blart.db");
    public static Config Config => Config.Default;
    public static Random Rng { get; } = new();

    public static void Main(string[] args)
    {
        Console.WriteLine($"Starting. Version: {Assembly.GetExecutingAssembly().GetName().Version}");
        _bot = new Bot(args);
        AppDomain.CurrentDomain.ProcessExit += (_, _) => _bot.Destroy();
    }
}*/