namespace BLART.Modules;

using System.Text;
using Discord.WebSocket;
using Objects;
using Services;

public static class ServerCountUpdater
{
    private static int _total = 0;
    private const ulong Channel1Id = 668983100889366545;
    private const ulong Channel2Id = 683825755888943190;
    
    private static string GetServerNameFromInfo(string info) => Encoding.UTF8.GetString(Convert.FromBase64String(info));
    
    public static async Task DoUpdate()
    {
        await Task.Delay(5000);
        
        for (;;)
        {
            Log.Info(nameof(DoUpdate), "Updating server channels..");
            try
            {
                Server[]? servers = await ServerListReader.GetAllServers();

                if (servers is null)
                {
                    Log.Error(nameof(DoUpdate), "Unable to retrieve servers.");
                    return;
                }

                int exiledCount = servers.Count(s => GetServerNameFromInfo(s.Info).Contains("Exiled"));
                double percent = Math.Floor((double) exiledCount / servers.Length * 100);
                string text = percent is 69 ? $"SEXILED Domination: {percent}%" : $"EXILED Domination: {percent}%";

                if (_total != 0 && _total == exiledCount)
                    return;

                _total = exiledCount;
                SocketVoiceChannel vc = Bot.Instance.Guild.GetVoiceChannel(Channel1Id);
                SocketVoiceChannel domination = Bot.Instance.Guild.GetVoiceChannel(Channel2Id);

                await vc.ModifyAsync(x => x.Name = $"Total EXILED Servers: {exiledCount}");
                await domination.ModifyAsync(x => x.Name = text);
            }
            catch (Exception e)
            {
                Log.Error(nameof(DoUpdate), e);
            }

            await Task.Delay(600000);
        }
    }
}
