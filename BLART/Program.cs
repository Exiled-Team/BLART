namespace BLART;

using Newtonsoft.Json;

public static class Program
{
    private static Config? _config;
    private static string KCfgFile = "Blart.json";

    public static string DatabaseFile { get; } = Path.Combine(Environment.CurrentDirectory, "Blart.db");
    public static Config Config => _config ??= GetConfig();
    public static Random Rng { get; } = new();
    public static void Main(string[] args) => new Bot(args);

    private static Config GetConfig()
    {
        if (File.Exists(KCfgFile))
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(KCfgFile))!;
        File.WriteAllText(KCfgFile, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
        return Config.Default;
    }
}