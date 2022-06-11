namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

public static class DictionaryExtensions
{
    /// <summary>
    /// Gets an item with a dictionary with a specified key or creates the item.
    /// </summary>
    /// <typeparam name="T1">Type of key.</typeparam>
    /// <typeparam name="T2">Type of value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">Key for the item.</param>
    public static T2 GetOrCreateItem<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key) where T2 : new()
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;

        var item = new T2();
        dictionary[key] = item;
        return item;
    }
}