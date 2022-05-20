namespace BloonsTD6.Mod.SalariedMonkeys.Interfaces;

/// <summary>
/// Represents an individual salaried tower.
/// </summary>
public interface ISalariedTower
{
    /// <summary>
    /// Returns the total cost of the tower.
    /// </summary>
    float GetTotalCost();

    /// <summary>
    /// Sells the tower in question.
    /// </summary>
    void Sell();
}

public static class SalariedTowerExtensions
{
    /// <summary>
    /// Returns the salary per round for this tower.
    /// </summary>
    /// <param name="tower">The tower in question.</param>
    /// <param name="settings">The settings for the mod.</param>
    public static float GetSalary(this ISalariedTower tower, ModSettings settings) => settings.CalculateCost(tower.GetTotalCost());
}