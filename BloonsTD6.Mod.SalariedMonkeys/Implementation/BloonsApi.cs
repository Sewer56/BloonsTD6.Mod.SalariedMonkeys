using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Unity.UI_New.InGame;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BloonsTD6.Mod.SalariedMonkeys.Utilities;
using BTD_Mod_Helper.Extensions;

namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

internal class BloonsApi : IBloonsApi
{
    public static readonly BloonsApi Instance = new BloonsApi();
    private static GlobalCostModModel? _cachedGlobalCostModel;

    public double GetCash() => InGame.instance.GetCash();

    public void AddCash(double amount) => InGame.instance.AddCash(amount);

    public List<ISalariedTower> GetTowers()
    {
        // We use this specific method because it doesn't include towers that have been destroyed.
        var towers = InGame.instance.GetAllTowerToSim();
        var list   = new List<ISalariedTower>();
        
        foreach (var tower in towers)
        {
            list.Add(new SalariedTower(tower.tower, SalariedMonkeys.Instance.GetTowerInfo(tower.tower)));
        }

        return list;
    }

    public bool? ToggleSelling(bool allowSelling)
    {
        var model = InGame.instance.GetGameModel();
        if (model == null)
            return default;

        var originalSellEnabled = model.towerSellEnabled;
        model.towerSellEnabled = allowSelling;
        return originalSellEnabled;
    }

    public bool IsRoundActive() => InGame.instance.UnityToSimulation.AreRoundsActive();

    public float GetDifficultyCostMultiplier() => GetCachedGlobalCostModel().multiplier;

    public static void ResetCacheForNewMatch() => _cachedGlobalCostModel = null;

    private static GlobalCostModModel GetCachedGlobalCostModel()
    {
        if (_cachedGlobalCostModel != null)
            return _cachedGlobalCostModel;

        var gameModel = InGame.instance.GetGameModel();
        var model     = gameModel.GetGlobalCostModel();
        if (model != null)
        {
            _cachedGlobalCostModel = model;
            return model;
        }
        
        ThrowHelpers.ThrowException("GlobalCostModModel not found");
        return null!;
    }
}