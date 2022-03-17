namespace BLART.Modules;

using System.Text;
using System.Text.RegularExpressions;

public class CatgirlShit
{
    private static List<string> joy { get; } = new()
    {
        " (* ^ ω ^)", " (o^▽^o)", " (≧◡≦)", " ☆⌒ヽ(*\"､^*)chu", " ( ˘⌣˘)♡(˘⌣˘ )", " xD"
    };

    private static List<string> embarassed { get; } = new()
    {
        " (⁄ ⁄>⁄ ▽ ⁄<⁄ ⁄)..", " (*^.^*)..,", "..,", ",,,", "... ", ".. ", " mmm..", "O.o"
    };

    private static List<string> confused { get; } = new()
    {
        " (o_O)?", " (°ロ°) !?", " (ーー;)?", " owo?"
    };

    private static List<string> messages { get; } = new()
    {
        "UwU, Poggie Woggies!",
        "<:catgirluppies:947415297994936330>",
        "<a:pepeshy:935067349730594836>",
        "What're the first 33 rules tho? <:awkwardstand:933623804603822111>"
    };

    public static Task<string> Uwu(string original)
    {
        string[] words = original.Split(' ');

        StringBuilder builder = new();
        foreach (string word in words)
        {
            char last = word[^1];
            string end = last.ToString();
            int random = 0;

            if (last == '.')
            {
                random = Program.Rng.Next(3);
                if (random == 0)
                    end = joy[Program.Rng.Next(joy.Count)];
            }
            else if (last == '?')
            {
                random = Program.Rng.Next(2);
                if (random == 0)
                    end = confused[Program.Rng.Next(confused.Count)];
            }
            else if (last == '!')
            {
                random = Program.Rng.Next(2);
                if (random == 0)
                    end = joy[Program.Rng.Next(joy.Count)];
            }
            else if (last == ',')
            {
                random = Program.Rng.Next(3);
                if (random == 0)
                    end = embarassed[Program.Rng.Next(embarassed.Count)];
            }

            StringBuilder uwu = new();
            for (int i = 0; i < word.Length; i++)
            {
                char current = word[i];
                char previous = i > 1 ? word[i - 1] : default;

                switch (current)
                {
                    case 'L' or 'R':
                        uwu.Append('W');
                        break;
                    case 'l' or 'r':
                        uwu.Append('w');
                        break;
                    case 'O' or 'o' when previous is 'N' or 'n' or 'M' or 'm':
                        uwu.Append("yo");
                        break;
                    case 'O' or 'o':
                        uwu.Append(current);
                        break;
                    default:
                        uwu.Append(current);
                        break;
                }
            }

            string uwuString = uwu.ToString().Replace(last.ToString(), end).Replace("you're", "ur")
                .Replace("youre", "ur").Replace("fuck", "fwickk").Replace("shit", "poo").Replace("bitch", "meanie")
                .Replace("dick", "peenie").Replace("penis", "peenie").Replace("asshole", "b-butthole")
                .Replace("cum", "cummies").Replace("dad", "daddy").Replace("father", "daddy");
            if (uwuString.Length > 2 && Regex.IsMatch(uwuString[0].ToString(), "/[a-z]/i", RegexOptions.IgnoreCase))
            {
                random = Program.Rng.Next(6);
                if (random == 0)
                    uwuString = uwuString[0] + "-" + uwuString;
            }

            builder.Append(uwuString);
            builder.Append(' ');
        }

        return Task.FromResult(builder.ToString());
    }
}