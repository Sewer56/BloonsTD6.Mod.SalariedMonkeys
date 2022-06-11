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
    /// Salary multiplier for rounds past 100.
    /// </summary>
    public float FreeplaySalaryMultiplier { get; set; } = 0.25f;

    /// <summary>
    /// If set to true, all forms of money generation are disabled.
    /// </summary>
    public bool DisableIncome { get; set; } = true;

    /// <summary>
    /// Includes destroyed towers in paragon sacrifice calculation.
    /// </summary>
    public bool IncludeDestroyedTowersInParagonDegree { get; set; } = true;

    /// <summary>
    /// Includes paragons when adding paragon degrees for destroyed towers.
    /// </summary>
    public bool IncludeParagonsInDestroyedTowersParagonDegree { get; set; } = true;

    /// <summary>
    /// Determines how selling is penalised in game.
    /// </summary>
    public SellPenaltyKind SellPenalty { get; set; } = SellPenaltyKind.FreeBetweenRounds;

    /// <summary>
    /// Calculates a new cost based on the modifiers specified in this settings page.
    /// </summary>
    /// <param name="baseCost">The original cost.</param>
    /// <param name="isFreeplay">Whether the player is in Freeplay mode or not.</param>
    public float CalculateCost(float baseCost, bool isFreeplay) => ApplyFreeplayMultiplier(baseCost * CostPercentPerRound, isFreeplay);

    /// <summary>
    /// Applies the freeplay multiplier to a given value.
    /// </summary>
    private float ApplyFreeplayMultiplier(float value, bool isFreeplay)
    {
        if (!isFreeplay)
            return value;

        return value * FreeplaySalaryMultiplier;
    }

    // It would be really nice to have 3rd party library access here so I could use Mapster.
    public void Map(in ModServerSettings other)
    {
        CostPercentPerRound = other.CostPercentPerRound;
        FreeplaySalaryMultiplier = other.FreeplaySalaryMultiplier;
        DisableIncome = other.DisableIncome;
        SellPenalty = other.SellPenalty;
        IncludeDestroyedTowersInParagonDegree = other.IncludeDestroyedTowersInParagonDegree;
        IncludeParagonsInDestroyedTowersParagonDegree = other.IncludeParagonsInDestroyedTowersParagonDegree;
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