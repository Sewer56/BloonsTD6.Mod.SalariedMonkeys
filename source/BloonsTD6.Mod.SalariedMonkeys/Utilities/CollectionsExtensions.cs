namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

public static class CollectionsExtensions
{
    /// <summary>
    /// Tries to pop an element from the stack if possible.
    /// </summary>
    /// <param name="stack">The stack to pop from.</param>
    /// <param name="value">Value obtained from the stack.</param>
    /// <returns>True on success, else false.</returns>
    public static bool TryPop<T>(this Stack<T> stack, out T? value)
    {
        if (stack.Count <= 0)
        {
            value = default;
            return false;
        }

        value = stack.Pop();
        return value != null;
    }
}