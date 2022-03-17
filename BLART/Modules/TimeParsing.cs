namespace BLART.Modules;

public class TimeParsing
{
    public static TimeSpan ParseDuration(string duration)
    {
        string[] parts = duration.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        TimeSpan span = new();

        foreach (string s in parts)
        {
            string digits = new(s.ToCharArray().Where(char.IsDigit).ToArray());
            if (!int.TryParse(digits, out int result))
                return TimeSpan.MinValue;

            char type = s.ToCharArray().FirstOrDefault(char.IsLetter);
            if (type == default(char))
                return TimeSpan.MinValue;

            span += FromChar(type, result);
        }

        return span;
    }

    public static TimeSpan FromChar(char c, int duration)
    {
        switch (c)
        {
            case 's':
                return TimeSpan.FromSeconds(duration);
            case 'm':
                return TimeSpan.FromMinutes(duration);
            case 'h':
                return TimeSpan.FromHours(duration);
            case 'd':
                return TimeSpan.FromDays(duration);
            case 'w':
                return TimeSpan.FromDays(duration * 7);
            case 'M':
                return TimeSpan.FromDays(duration * 30);
            case 'y':
                return TimeSpan.FromDays(duration * 365);
            default:
                return TimeSpan.MinValue;
        }
    }
}