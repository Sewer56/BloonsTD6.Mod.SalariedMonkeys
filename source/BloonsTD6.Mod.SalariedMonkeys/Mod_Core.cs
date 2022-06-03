using Assets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Simulation.Towers.Pets;
using TowerManager = BloonsTD6.Mod.SalariedMonkeys.Implementation.TowerManager;

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to the mod's core functionality.
/// </summary>
public partial class Mod
{
    private void OnMatchEnd_Core() => SalariedMonkeys.DeInitialize();

    // We inject into the cash add function because Co-Op sends out a synchronization message
    // before the round end call, which makes syncing during round end more dangerous.
    public static void AfterAddEndOfRoundCash()
    {
        Log.Debug("AfterEndOfRoundCash");
        InGame.instance.GetPlayerIndices().ForEachTrue(x => SalariedMonkeys.PaySalaries(x));
    }

    // Sell towers when the round ends.
    public override void OnRoundEnd()
    {
        Log.Debug("OnRoundEnd");
        InGame.instance.GetPlayerIndices().ForEachTrue(x => SalariedMonkeys.SellTowers(x));

#if DEBUG
        var towers = SalariedMonkeys.Api.GetTowers(0);
        var totalCost = towers.Sum(x => x.GetTotalCost());
        MelonLogger.Msg($"Total Tower Cost: {totalCost}");
#endif
    }

    // Hooks for updating cash display.
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        Log.Debug("TowerCreated");
        _invalidateCashDisplayTimer = 3;
    }

    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        Log.Debug("TowerUpgraded");
        _invalidateCashDisplayTimer = 3;
    }

    public override void OnTowerDestroyed(Tower tower)
    {
        // Make the user pay for the tower for the round if selling before round end.
        Log.Debug("TowerDestroyed");
        SalariedMonkeys.OnSellTower(tower);
        _invalidateCashDisplayTimer = 3;
    }

    // Disable income in other modes.
    public static void OnCreateModded(GameModel result, Il2CppSystem.Collections.Generic.List<ModModel> mods)
    {
        if (!_modSettings.DisableIncome)
            return;

        result.DisableIncome();
        result.RemoveTower(TowerType.BananaFarm);
    }

    // Initialize GameMode
    public static void AfterCreateModded(GameModel result, Il2CppSystem.Collections.Generic.List<ModModel> mods)
    {
        SalariedMonkeys.ConstructInGame(new TowerManager(BloonsApi.Instance, _modSettings), result);
    }

    // Tower Specific Patches 
    #region Adora
    public static Stack<(Tower, float)> _adoraRestoreTowerWorthStack = new();

    public static bool OnAdoraBloodSacrifice(BloodSacrifice bloodSacrifice)
    {
        Log.Debug($"[Blood Sacrifice] Tower: {bloodSacrifice.selectedTowerId} | Restoring Cost For sacrifice.");
        var tower = InGame.instance.GetTowerById(bloodSacrifice.selectedTowerId);
        if (tower == null)
            return Log.Debug(false, "Blood Sacrifice Tower Not Found!!");

        _adoraRestoreTowerWorthStack.Push((tower, tower.worth));
        var towerInfo = SalariedMonkeys.GetTowerInfo(tower);
        tower.worth   = towerInfo.CalculateCostWithDiscounts(Api, tower);
        return Log.Debug(true, $"New Temp Tower Worth: {tower.worth}");
    }

    public static void AfterAdoraBloodSacrifice(BloodSacrifice bloodSacrifice)
    {
        Log.Debug($"[Blood Sacrifice] Tower: {bloodSacrifice.selectedTowerId} | Re-setting cost to 0.");
        if (_adoraRestoreTowerWorthStack.TryPop(out var twr))
            twr.Item1.worth = twr.Item2;
    }
    #endregion

    #region Paragon
    public static Stack<(Tower, float)> _paragonRestoreTowerWorthStack = new();

    public static void OnParagonGetTowerInvestment(ParagonTower paragonTower, Tower towerToUse)
    {
        _paragonRestoreTowerWorthStack.Push((towerToUse, towerToUse.worth));
        var towerInfo    = SalariedMonkeys.GetTowerInfo(towerToUse);
        towerToUse.worth = towerInfo.CalculateCostWithDiscounts(Api, towerToUse);
        Log.Debug($"Paragon: New Temp Tower Worth: {towerToUse.worth}");
    }

    public static void AfterParagonGetTowerInvestment(ParagonTower paragonTower, Tower towerToUse)
    {
        Log.Debug($"[Paragon] Tower: {towerToUse.Id} | Re-setting cost to 0.");
        if (_paragonRestoreTowerWorthStack.TryPop(out var twr))
            twr.Item1.worth = twr.Item2;
    }
    #endregion
}