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

    public static void OnSellTower(Tower tower) => SalariedMonkeys.OnSellTower(tower);

    public override void OnTowerDestroyed(Tower tower)
    {
        Log.Debug($"TowerDestroyed, Worth {tower.worth}");
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
}