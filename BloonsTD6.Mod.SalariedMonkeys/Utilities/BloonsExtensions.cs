using Assets.Scripts.Models;
using Assets.Scripts.Models.Difficulty;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using BTD_Mod_Helper.Extensions;

namespace BloonsTD6.Mod.SalariedMonkeys.Utilities;

internal static class BloonsExtensions
{
    /// <summary>
    /// Obtains the global cost model for the current difficulty.
    /// </summary>
    /// <param name="model">Access to the game instance.</param>
    public static GlobalCostModModel? GetGlobalCostModel(this GameModel model)
    {
        var difficultyName = model.difficultyId;
        var modModel = model.GetModModel(difficultyName);
        var costModel = modModel?.GetChild<GlobalCostModModel>();
        return costModel;
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
}