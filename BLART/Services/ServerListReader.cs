namespace BLART.Services;

using Newtonsoft.Json;
using Objects;

public static class ServerListReader
{
    private static HttpClient HttpClient = new();

    public static async Task<Server[]?> GetAllServers()
    {
        HttpRequestMessage message = new();
        message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2");
        message.Method = HttpMethod.Get;
        message.RequestUri =
            new($"https://api.scpslgame.com/lobbylist.php?key={Program.Config.NorthwoodApiKey}&minimal&format=json");
        HttpResponseMessage response = await HttpClient.SendAsync(message);
        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"Operation failed to execute with error code {response.StatusCode} and reason: {response.ReasonPhrase}");
        string json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Server[]>(json);
    }
}