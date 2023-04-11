namespace BLART.Modules;

public class ReasonParsing
{
    public static string ParseRules(string reason)
    {
        return reason.Replace("[0]", "Disruptive Behavior").Replace("[1]", "Rule 1 - Racist/Derogatory Language ")
            .Replace("[2]", "Rule 2 - Spamming ").Replace("[3]", "Rule 3 - Malicious Links. ")
            .Replace("[4]", "Rule 4 - Posting in inappropriate channels. ")
            .Replace("[5]", "Rule 5 - Malicious Activity. ").Replace("[6]", "Rule 6 - Command Abuse")
            .Replace("[7]", "Rule 7 - Non-english in English only channels. ")
            .Replace("[8]", "Rule 8 - Advertisement. ")
            .Replace("[9]", "Rule 9 - Do not ask questions related to ZAP hosting. ")
            .Replace("[10]", "Rule 10 - Unnecessarily pinging Joker. ").TrimEnd(' ');
    }
}