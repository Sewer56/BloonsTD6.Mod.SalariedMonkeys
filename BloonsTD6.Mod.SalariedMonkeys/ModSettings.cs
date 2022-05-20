namespace BloonsTD6.Mod.SalariedMonkeys;

public class ModSettings
{
    /// <summary>
    /// The fraction of tower cost subtracted at the end of each round.
    /// Value of 0.05 indicates 5%.
    /// </summary>
    public float CostPercentPerRound = 0.05f;

    /// <summary>
    /// Calculates a new cost based on the modifiers specified in this settings page.
    /// </summary>
    /// <param name="baseCost">The original cost.</param>
    public float CalculateCost(float baseCost) => baseCost * CostPercentPerRound;
}