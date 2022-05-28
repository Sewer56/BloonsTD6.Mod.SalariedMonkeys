using BTD_Mod_Helper.Api.ModOptions;

namespace BloonsTD6.Mod.SalariedMonkeys;

public class ModSettings
{
    /// <summary>
    /// The fraction of tower cost subtracted at the end of each round.
    /// Value of 0.05 indicates 5%.
    /// </summary>
    public float CostPercentPerRound = 0.05f;

    /// <summary>
    /// If set to true, all forms of money generation are disabled.
    /// </summary>
    public bool DisableIncome = true;

    /// <summary>
    /// Calculates a new cost based on the modifiers specified in this settings page.
    /// </summary>
    /// <param name="baseCost">The original cost.</param>
    /// <param name="difficultyCostMultiplier">The cost multiplier for the current difficulty.</param>
    public float CalculateCost(float baseCost, float difficultyCostMultiplier = 1.00f) => (baseCost * CostPercentPerRound) * difficultyCostMultiplier;
}