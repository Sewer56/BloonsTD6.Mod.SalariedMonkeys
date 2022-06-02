namespace BloonsTD6.Mod.SalariedMonkeys.Structures;

/// <summary>
/// Packs 64 booleans inside a single long.
/// </summary>
public struct PackedBoolArray
{
    /// <summary>
    /// Size of the complete packed array,
    /// </summary>
    public const int ArraySize = 64;

    private long _value = 0;

    /// <summary>
    /// Initializes the packed boolean array with a default value.
    /// </summary>
    public PackedBoolArray(long value) => _value = value;

    /// <summary>
    /// Size of the array.
    /// </summary>
    public int Length => ArraySize;

    /// <summary>
    /// Checks if a bool at a given value is true.
    /// </summary>
    /// <param name="index">Zero based index of the bool.</param>
    public bool GetValue(int index) => ((_value >> index) & 1) == 1;

    /// <summary>
    /// Sets the value at a specified index.
    /// </summary>
    /// <param name="index">Index to set the value at.</param>
    /// <param name="value">The value in question.</param>
    public void SetValue(int index, bool value)
    {
        var valueToSet = Convert.ToInt64(value) << index;
        var mask       = ~(1L << index);
        _value = (_value & mask) | valueToSet;
    }

    /// <summary>
    /// Returns true if this array is empty/zero.
    /// </summary>
    public bool IsEmpty() => _value == 0;

    /// <summary>
    /// Runs a piece of code for each true value.
    /// </summary>
    /// <param name="action">Action to execute. Parameter is array index.</param>
    public void ForEachTrue(Action<int> action)
    {
        for (int x = 0; x < Length; x++)
        {
            if (GetValue(x))
                action(x);
        }
    }
}