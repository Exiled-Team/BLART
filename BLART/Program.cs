namespace BLART;

using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

public static class Program
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
}