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
                double percent = (double) exiledCount / servers.Length * 100;
                string text = $"EXILED Domination: {Math.Floor(percent)}%";

                if (_total != 0 && _total == exiledCount)
                    return;

                _total = exiledCount;
                SocketVoiceChannel vc = Bot.Instance.Guild.GetVoiceChannel(Channel1Id);
                SocketVoiceChannel domination = Bot.Instance.Guild.GetVoiceChannel(Channel2Id);

                await vc.ModifyAsync(x => x.Name = $"Total EXILED Servers: {exiledCount}", new() { AuditLogReason = "Update Exiled server count." });
                await domination.ModifyAsync(x => x.Name = text, new() { AuditLogReason = "Update Exiled server count." });
            }
            catch (Exception e)
            {
                Log.Error(nameof(DoUpdate), e);
            }

            await Task.Delay(600000);
        }
    }
}