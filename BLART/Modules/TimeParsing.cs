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
        return c switch
        {
            's' => TimeSpan.FromSeconds(duration),
            'm' => TimeSpan.FromMinutes(duration),
            'h' => TimeSpan.FromHours(duration),
            'd' => TimeSpan.FromDays(duration),
            'w' => TimeSpan.FromDays(duration * 7),
            'M' => TimeSpan.FromDays(duration * 30),
            'y' => TimeSpan.FromDays(duration * 365),
            _ => TimeSpan.MinValue
        };
    }
}