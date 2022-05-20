namespace BloonsTD6.Mod.SalariedMonkeys.Interfaces;

/// <summary>
/// Abstracts a set of common game utility functions.
/// </summary>
public interface IBloonsApi
{
    /// <summary>
    /// Returns the amount of money available.
    /// </summary>
    double GetCash();

    /// <summary>
    /// Adds an amount of cash to the game.
    /// </summary>
    /// <param name="amount">The amount of cash to add.</param>
    void AddCash(double amount);

    /// <summary>
    /// Returns a list of currently placed towers.
    /// </summary>
    List<ISalariedTower> GetTowers();

    /// <summary>
    /// Sells a given tower in question.
    /// </summary>
    /// <param name="tower">The tower to sell.</param>
    void SellTower(ISalariedTower tower);
}