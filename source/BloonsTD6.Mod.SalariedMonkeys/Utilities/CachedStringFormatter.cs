using System.Diagnostics.CodeAnalysis;

namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

/// <summary>
/// String formatter that caches the resultant strings.
/// To be used until I find more efficient ways to change menu texts without spamming the GC.
/// </summary>
[ExcludeFromCodeCoverage] // doesn't require testing
internal class CachedStringFormatter
{
    private Dictionary<float, string> _floatToWithDollarDictionary = new Dictionary<float, string>();
    private Dictionary<float, string> _floatToCostDictionary = new Dictionary<float, string>();
    private Dictionary<float, string> _floatToSalaryDictionary = new Dictionary<float, string>();

    /// <summary>
    /// Gets a string representing the upgrade cost for a tower.
    /// The upgrade cost is simply represented as a raw numeric value.
    /// </summary>
    /// <param name="value">The value to get string for.</param>
    public string GetUpgradeCostWithDollar(float value)
    {
        if (_floatToWithDollarDictionary.TryGetValue(value, out string text))
            return text;

        text = $"${value:####0.#}";
        _floatToWithDollarDictionary[value] = text;
        return text;
    }

    /// <summary>
    /// Gets a string representing the upgrade cost for a tower.
    /// The upgrade cost is simply represented as a raw numeric value.
    /// </summary>
    /// <param name="value">The value to get string for.</param>
    public string GetUpgradeCost(float value)
    {
        if (_floatToCostDictionary.TryGetValue(value, out string text))
            return text;

        text = value.ToString("####0.#");
        _floatToCostDictionary[value] = text;
        return text;
    }

    /// <summary>
    /// Gets a string representing the salary to append to the cash counter.
    /// </summary>
    /// <param name="value">The value to get string for.</param>
    public string GetSalary(float value)
    {
        if (_floatToSalaryDictionary.TryGetValue(value, out string text))
            return text;

        text = $" (-{value:####0.#})";
        _floatToSalaryDictionary[value] = text;
        return text;
    }

    public void Clear()
    {
        _floatToCostDictionary.Clear();
        _floatToSalaryDictionary.Clear();
        _floatToWithDollarDictionary.Clear();
    }
}