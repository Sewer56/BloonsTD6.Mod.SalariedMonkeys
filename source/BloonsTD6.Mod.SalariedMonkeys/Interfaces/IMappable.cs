namespace BloonsTD6.Mod.SalariedMonkeys.Interfaces;

/// <summary>
/// Interface that exports the ability to map interfaces to itself.
/// </summary>
public interface IMappable<TSelf>
{
    /// <summary>
    /// Maps another instance of self onto itself.
    /// </summary>
    /// <param name="other">The object whose values to copy to itself.</param>
    void Map(in TSelf other);
}

public static class IMappableExtensions
{
    /// <summary>
    /// Clones a mappable object.
    /// </summary>
    public static TSelf Clone<TSelf>(this TSelf mappable) where TSelf : IMappable<TSelf>, new()
    {
        var result = new TSelf();
        result.Map(mappable);
        return result;
    }
}