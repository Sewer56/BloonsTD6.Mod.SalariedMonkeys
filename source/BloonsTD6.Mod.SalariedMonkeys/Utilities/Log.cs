namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

public static class Log
{
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Debug(string text) => MelonLogger.Msg(text);

    public static void Always(string text) => MelonLogger.Msg(text);

    public static void NoMelon(string text) => Console.WriteLine(text);

    public static void NoMelon(ConsoleColor color, string text)
    {
        using var colorSetter = new TemporarySetConsoleColour(color);
        Console.WriteLine(text);
    }
    public static void NoMelonNoLine(ConsoleColor color, string text)
    {
        using var colorSetter = new TemporarySetConsoleColour(color);
        Console.Write(text);
    }

    public static bool Debug(bool result, string text)
    {
        Debug(text);
        return result;
    }

    public static bool Always(bool result, string text)
    {
        MelonLogger.Msg(text);
        return result;
    }
}

public struct TemporarySetConsoleColour : IDisposable
{
    private ConsoleColor _original;

    public TemporarySetConsoleColour(ConsoleColor newColor)
    {
        _original = Console.ForegroundColor;
        Console.ForegroundColor = newColor;
    }

    public void Dispose() => Console.ForegroundColor = _original;
}