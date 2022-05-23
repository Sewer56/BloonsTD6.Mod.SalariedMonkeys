using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers.Mods;

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
}