﻿namespace BloonsTD6.Mod.SalariedMonkeys.Interfaces;

/// <summary>
/// Interface that manages all the towers controlled by Monkeys for Hire.
/// </summary>
public interface ITowerManager
{
    /// <summary>
    /// The API implementation used to purchase/sell stuff in Bloons.
    /// </summary>
    IBloonsApi BloonsApi { get; set; }

    /// <summary>
    /// Settings of the mod itself.
    /// </summary>
    ModClientSettings Settings { get; set; }

    /// <summary>
    /// Returns the amount of payable money to the monkeys.
    /// </summary>
    /// <param name="playerIndex">Index of the player to</param>
    /// <param name="increaseTowerWorth">Increases the tower instances' total worth.</param>
    double GetTotalSalary(int playerIndex, bool increaseTowerWorth = false);

    /// <summary>
    /// Sells all towers, starting from cheapest tower until salary demand is met.
    /// </summary>
    /// <param name="playerIndex">The index for which to sell towers for.</param>
    /// <param name="requiredSalary">The required amount of money to be made.</param>
    /// <remarks>To be used in co-op!</remarks>
    /// <returns>Amount of new payable salary gained.</returns>
    double SellTowers(int playerIndex, float requiredSalary);
}