using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Difficulty;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Simulation.SMath;
using Assets.Scripts.Simulation.Towers.Behaviors;
using BTD_Mod_Helper.Extensions;

namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

[ExcludeFromCodeCoverage] // Only way to test these is in-game.
internal static class BloonsExtensions
{
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
        if (model.difficultyId == ModeType.CHIMPS)
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