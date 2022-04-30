namespace BLART;

using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

public static class Program
{
    private static Config? _config;
    private static string KCfgFile = "Blart.json";
    private static Bot? _bot;

    public static string DatabaseFile { get; } = Path.Combine(Environment.CurrentDirectory, "Blart.db");
    public static Config Config => _config ??= GetConfig();
    public static Random Rng { get; } = new();

    public static void Main(string[] args)
    {
        Console.WriteLine($"Starting. Version: {Assembly.GetExecutingAssembly().GetName().Version}");
        if (args.Contains("--debug"))
            Config.Debug = true;
        _bot = new Bot(args);
        AppDomain.CurrentDomain.ProcessExit += (_, _) => _bot.Destroy();
    }

    private static Config GetConfig()
    {
        if (File.Exists(KCfgFile))
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(KCfgFile))!;
        File.WriteAllText(KCfgFile, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
        return Config.Default;
    }
}