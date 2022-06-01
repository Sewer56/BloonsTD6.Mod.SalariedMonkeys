namespace BloonsTD6.Mod.SalariedMonkeys.Interfaces;

/// <summary>
/// Abstracts a set of common game utility functions.
/// </summary>
public interface IBloonsApi
{
    /// <summary>
    /// Gets the zero based player index used for co-op.
    /// Can also be used for single player APIs.
    /// </summary>
    int GetPlayerIndex();

    /// <summary>
    /// Returns the amount of money available.
    /// </summary>
    /// <param name="playerIndex">Index of the player to get cash from.</param>
    double GetCash(int playerIndex);

    /// <summary>
    /// Adds an amount of cash to the game.
    /// </summary>
    /// <param name="amount">The amount of cash to add.</param>
    /// <param name="playerIndex">Index of the player to add/remove cash to/from.</param>
    void AddCash(double amount, int playerIndex);

    /// <summary>
    /// Returns a list of currently placed towers for a given player.
    /// </summary>
    /// <param name="playerIndex">Index of the player to get towers for.</param>
    List<ISalariedTower> GetTowers(int playerIndex);

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

public static class IBloonsApiExtensions
{
    /// <summary>
    /// Returns true if a given player owns a tower.
    /// </summary>
    public static bool PlayerOwnsTower(this IBloonsApi api, Tower tower, int zeroBasedIndex) => tower.GetOwnerZeroBased() == zeroBasedIndex;

    /// <summary>
    /// Returns true if the current player owns a tower.
    /// </summary>
    public static bool PlayerOwnsTower(this IBloonsApi api, Tower tower)
    {
        return api.PlayerOwnsTower(tower, api.GetPlayerIndex());
    }
}