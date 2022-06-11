namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

public static class Il2CppDictionaryExtensions
{
    /// <summary>
    /// Managed replacement for TryGetValue, since the unhollower one's broken.
    /// </summary>
    public static bool TryGetValue<T1, T2>(this Il2CppSystem.Collections.Generic.Dictionary<T1, T2> dictionary, T1 key, out T2 value)
    {
        try
        {
            value = dictionary[key];
            return true;
        }
        catch (Exception)
        {
            value = default!;
            return false;
        }
    }
}