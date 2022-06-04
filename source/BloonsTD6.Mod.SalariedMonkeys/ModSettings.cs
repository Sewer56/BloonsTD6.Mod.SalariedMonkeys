namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// User specific settings.
/// Unrelated to other players.
/// </summary>
public class ModClientSettings : ModSharedSettings, IMappable<ModClientSettings>
{
    /// <summary>
    /// Shows the salary indicator in the 
    /// </summary>
    public bool ShowSalaryInUI { get; set; } = true;

    /// <summary>
    /// Changes how the sell price of the tower is displayed on the UI.
    /// </summary>
    public SellPriceDisplayMode SellPriceDisplayMode { get; set; } = SellPriceDisplayMode.SalaryOnly;

    // It would be really nice to have 3rd party library access here so I could use Mapster.
    public void Map(in ModClientSettings other)
    {
        ShowSalaryInUI = other.ShowSalaryInUI;
        SellPriceDisplayMode = other.SellPriceDisplayMode;
        base.Map(other);
    }
}

/// <summary>
/// Settings required for correct functioning (e.g. client skins) necessary for
/// synchronization but not necessary for correctness.
/// </summary>
public class ModSharedSettings : ModServerSettings, IMappable<ModSharedSettings>
{

    // It would be really nice to have 3rd party library access here so I could use Mapster.
    public void Map(in ModSharedSettings other)
    {
        base.Map(other);
    }
}

/// <summary>
/// Settings that need to be synced for all players.
/// </summary>
public class ModServerSettings : IMappable<ModServerSettings>
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

    // It would be really nice to have 3rd party library access here so I could use Mapster.
    public void Map(in ModServerSettings other)
    {
        CostPercentPerRound = other.CostPercentPerRound;
        DisableIncome = other.DisableIncome;
        SellPenalty = other.SellPenalty;
    }
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

public enum SellPriceDisplayMode
{
    TowerWorth,
    SalaryOnly,
    TowerWorthAndSalary,
}