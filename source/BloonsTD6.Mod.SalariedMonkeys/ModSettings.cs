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
    /// Determines how selling is penalised in game.
    /// </summary>
    public SellPenaltyKind SellPenalty = SellPenaltyKind.FreeBetweenRounds;

    /// <summary>
    /// Calculates a new cost based on the modifiers specified in this settings page.
    /// </summary>
    /// <param name="baseCost">The original cost.</param>
    public float CalculateCost(float baseCost) => (baseCost * CostPercentPerRound);
}

public enum SellPenaltyKind
{
    /// <summary>
    /// Selling always incurs a cost penalty.
    /// </summary>
    Always,

    /// <summary>
    /// Selling is free between rounds, otherwise costs money.
    /// </summary>
    FreeBetweenRounds,

    /// <summary>
    /// Selling is always free and costs no money.
    /// </summary>
    Free,
}