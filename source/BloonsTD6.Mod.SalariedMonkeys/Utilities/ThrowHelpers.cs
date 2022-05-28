namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

/// <summary>
/// Used to allow methods throwing exceptions to be inlined.
/// </summary>
internal static class ThrowHelpers
{
    public static void ThrowException(string message) => throw new Exception(message);
}