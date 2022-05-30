using UnhollowerBaseLib;

namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

internal struct Il2CppNativeHookVariable<TType> where TType : Il2CppObjectBase
{
    /// <summary>
    /// Pointer to the raw object in memory.
    /// </summary>
    public IntPtr RawValue { get; set; }

    /// <summary>
    /// Gets the abstracted type using the pointer.
    /// </summary>
    public TType GetObject()
    {
        var obj = new Il2CppObjectBase(RawValue);
        return obj.Cast<TType>();
    }
}