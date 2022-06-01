namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// User specific settings.
/// Unrelated to other players.
/// </summary>
public class ModClientSettings : ModSharedSettings
{
    /// <summary>
    /// Shows the salary indicator in the 
    /// </summary>
    public bool ShowSalaryInUI { get; set; } = true;
}

/// <summary>
/// Settings required for correct functioning (e.g. client skins) necessary for
/// synchronization but not necessary for correctness.
/// </summary>
public class ModSharedSettings : ModServerSettings
{

}

/// <summary>
/// Settings that need to be synced for all players.
/// </summary>
public class ModServerSettings
{
    /// <summary>
    /// The fraction of tower cost subtracted at the end of each round.
    /// Value of 0.05 indicates 5%.
    /// </summary>
    public float CostPercentPerRound { get; set; } = 0.05f;

    /// <summary>
    /// If set to true, all forms of money generation are disabled.
    /// </summary>
    public bool DisableIncome { get; set; } = true;

    /// <summary>
    /// Determines how selling is penalised in game.
    /// </summary>
    public SellPenaltyKind SellPenalty { get; set; } = SellPenaltyKind.FreeBetweenRounds;

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