namespace BLART.Modules;

using System.Drawing;
using System.Globalization;

public class ColorParsing
{
    public static int ToColorValue(string s) => int.Parse(ToHexString(ColorTranslator.FromHtml($"#{s}")).Replace("#", string.Empty), NumberStyles.HexNumber);
    private static string ToHexString(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}