namespace BloonsTD6.Mod.SalariedMonkeys.Interfaces;

/// <summary>
/// Represents an individual salaried tower.
/// </summary>
public interface ISalariedTower
{
    /// <summary>
    /// Get information for a given salaried tower.
    /// </summary>
    TowerInfo GetTowerInfo();

    /// <summary>
    /// Returns the total cost of the tower.
    /// This includes discounts.
    /// </summary>
    float GetTotalCost();

    /// <summary>
    /// Sells the tower in question.
    /// </summary>
    void Sell();

    /// <summary>
    /// Increases the total worth of the tower by a set amount.
    /// </summary>
    /// <param name="amount">The amount to increase the worth by.</param>
    void IncreaseWorth(float amount);
}

public static class SalariedTowerExtensions
{
    /// <summary>
    /// Returns the salary per round for this tower.
    /// </summary>
    /// <param name="tower">The tower in question.</param>
    /// <param name="isFreeplay">True if the player is in freeplay mode, else false.</param>
    /// <param name="settings">The settings for the mod.</param>
    public static float GetSalary(this ISalariedTower tower, bool isFreeplay, ModClientSettings settings) => settings.CalculateCost(tower.GetTotalCost(), isFreeplay);
}