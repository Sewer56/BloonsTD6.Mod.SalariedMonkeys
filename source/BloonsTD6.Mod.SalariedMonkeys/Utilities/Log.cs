namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

public static class Log
{
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Debug(string text) => MelonLogger.Msg(text);
}