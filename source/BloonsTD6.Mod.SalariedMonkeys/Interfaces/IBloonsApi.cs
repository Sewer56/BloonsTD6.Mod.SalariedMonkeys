using Assets.Scripts.Simulation.SMath;
using Assets.Scripts.Simulation.Towers.Behaviors;

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
    /// Toggles whether selling is enabled or not.
    /// </summary>
    /// <returns>Original value declaring if selling is allowed or not.</returns>
    bool? ToggleSelling(bool allowSelling);

    /// <summary>
    /// True if currently active in a round, else false.
    /// </summary>
    bool IsRoundActive();

    /// <summary>
    /// Retrieves the info of all the discounts applicable to a given position on the map.
    /// </summary>
    /// <param name="position">Position of the tower.</param>
    /// <param name="path">Path the tower lies on.</param>
    /// <param name="tier">The tier of the tower.</param>
    /// <returns></returns>
    Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<DiscountZone>> GetDiscountInfo(Vector3 position, int path, int tier);
}