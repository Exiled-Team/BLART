namespace BLART.Objects;

public struct Server
{
    public int ServerId { get; set; }

    public int AccountId { get; set; }

    public string Ip { get; set; }

    public int Port { get; set; }

    public string Players { get; set; }

    public int Distance { get; set; }

    public string Info { get; set; }

    public string Pastebin { get; set; }

    public string Version { get; set; }

    public bool PrivateBeta { get; set; }

    public bool FriendlyFire { get; set; }

    public bool Modded { get; set; }

    public bool Whitelist { get; set; }

    public string IsoCode { get; set; }

    public string ContinentCode { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Official { get; set; }

    public int OfficialCode { get; set; }

    public int DisplaySection { get; set; }
}