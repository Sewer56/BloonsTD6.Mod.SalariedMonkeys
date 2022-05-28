using Assets.Scripts.Models;
using Assets.Scripts.Models.Difficulty;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Unity.UI_New.InGame.RightMenu;
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

    /// <summary>
    /// Removes the banana farm from the shop.
    /// After calling this, consider calling shop.TowerSetChanged(true);
    /// </summary>
    /// <param name="shop">The shop to remove item from.</param>
    /// <param name="towerId">ID of the tower to remove from shop.</param>
    public static void RemoveTowerButton(this ShopMenu shop, string towerId)
    {
        for (int x = shop.ActiveTowerButtons.Count - 1; x >= 0; x--)
        {
            var item = shop.activeTowerButtons[x];
            if (item.towerModel.baseId == towerId)
                shop.ActiveTowerButtons.Remove(item);
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
}