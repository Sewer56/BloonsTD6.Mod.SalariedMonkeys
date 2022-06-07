namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

[ExcludeFromCodeCoverage] // Only way to test these is in-game.
internal static class BloonsExtensions
{
    /// <summary>
    /// Returns a packed boolean array which denote which players are available
    /// in the current game. Entries are zero indexed.
    /// </summary>
    public static PackedBoolArray GetPlayerIndices(this InGame inGame)
    {
        if (!inGame.IsCoop)
            return new PackedBoolArray(1); // 0th element true

        var players = inGame.coopGame.AllPlayers.GetEnumeratorCollections();
        var result  = new PackedBoolArray();

        while (players.MoveNext())
        {
            var player = players.Current.Cast<CoopPlayerInfo>();
            result.SetValue(player.PlayerNumber - 1, true);
        }

        return result;
    }

    /// <summary>
    /// Gets the zero based index of the player that owns the tower.
    /// Returns P1 for unknown towers.
    /// </summary>
    public static int GetOwnerZeroBased(this Tower tower)
    {
        const int TOWER_NO_OWNER = -1;
        if (tower.owner == TOWER_NO_OWNER)
            return 0;

        return tower.owner - 1;
    }

    /// <summary>
    /// Gets all descendants and clones them, returning a mapping from old value to new value.
    /// </summary>
    /// <typeparam name="T">The type of descendant to clone.</typeparam>
    /// <param name="model">The model to clone the descendants from.</param>
    public static Dictionary<T, T> CloneDescendants<T>(this GameModel model) where T : Model
    {
        var result   = new Dictionary<T, T>();
        var desc     = model.GetDescendants<T>();
        var descEnum = desc.GetEnumeratorCollections();
        while (descEnum.MoveNext())
        {
            var item = descEnum.Current.Cast<T>();
            var cloned = item.Clone().Cast<T>();
            result[item] = cloned;
        }

        return result;
    }

    /// <summary>
    /// Mutates the current game model to disable income.
    /// </summary>
    /// <param name="model">The current game model.</param>
    public static void DisableIncome(this GameModel model)
    {
        if (model.gameMode == ModeType.CHIMPS)
            return;

        // Disable Income
        var bonusLivesModels = model.CloneDescendants<BonusLivesPerRoundModel>();
        var healthyBananasModels = model.CloneDescendants<HealthyBananasModModel>();

        var chimps = new ChimpsModModel("Fake Chimps");
        chimps.Mutate(model, model);

        foreach (var modelPair in bonusLivesModels)
        {
            modelPair.Key.amount = modelPair.Value.amount;
            modelPair.Key.assetId = modelPair.Value.assetId;
            modelPair.Key.lifespan = modelPair.Value.lifespan;
        }

        foreach (var modelPair in healthyBananasModels)
        {
            modelPair.Key.centralMarketLives = modelPair.Value.centralMarketLives;
            modelPair.Key.marketplaceLives = modelPair.Value.marketplaceLives;
            modelPair.Key.displayLifespan = modelPair.Value.displayLifespan;
            modelPair.Key.displayPath = modelPair.Value.displayPath;
        }
    }

    /// <summary>
    /// Removes a tower from a list of all towers in the GameModel.
    /// </summary>
    /// <param name="model">The model to remove the ID from.</param>
    /// <param name="towerId">Unique ID of the tower.</param>
    public static void RemoveTower(this GameModel model, string towerId)
    {
        model.towerSet = model.towerSet.Where(x => x.towerId != towerId).ToIl2CppReferenceArray();
    }

    /// <summary>
    /// Converts a boxed vector to our own.
    /// Re-implemented in managed code to save performance.
    /// </summary>
    public static Vector3 ToVector3(this Vector3Boxed boxed) => new(boxed.X, boxed.Y, boxed.Z);

    /// <summary>
    /// Returns the total discount multiplier for a set of discount zones.
    /// </summary>
    /// <param name="zones">The discount zones in question.</param>
    public static float CalcTotalDiscountMultiplier(this Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<DiscountZone>> zones)
    {
        var totalDiscount = 1.0f;

        foreach (var discount in zones)
        foreach (var value in discount.Value)
            totalDiscount -= value.discountZoneModel.discountMultiplier;

        return totalDiscount;
    }
}