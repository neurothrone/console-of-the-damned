namespace ConsoleOfTheDamned.Terminal.Utils;

public static class Colors
{
    public const string Reset = "\u001b[0m";
    public const string Red = "\u001b[31m";         // deep red (#B22222)
    public const string Purple = "\u001b[35m";      // purple/violet (#800080)
    public const string BrightWhite = "\u001b[97m"; // bright white
    public const string Gray = "\u001b[37m";        // light gray

    private static string Colorize(string text, string color) => color + text + Reset;

    public static string Warning(string text) => Colorize(text, Red);
    public static string Arcane(string text) => Colorize(text, Purple);
    public static string Highlight(string text) => Colorize(text, BrightWhite);
    public static string Flavor(string text) => Colorize(text, Gray);
}