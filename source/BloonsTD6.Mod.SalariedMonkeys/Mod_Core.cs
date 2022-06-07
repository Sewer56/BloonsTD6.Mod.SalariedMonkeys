using TowerManager = BloonsTD6.Mod.SalariedMonkeys.Implementation.TowerManager;

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to the mod's core functionality.
/// </summary>
public partial class Mod
{
    private void OnMatchEnd_Core() => SalariedMonkeys.DeInitialize();

    // Sell towers when the round ends.
    public void OnRoundEnd_Core()
    {
        InGame.instance.GetPlayerIndices().ForEachTrue(x => SalariedMonkeys.SellTowers(x));
    }

    // We inject into the cash add function because Co-Op sends out a synchronization message
    // before the round end call, which makes syncing during round end more dangerous.
    public static void AfterAddEndOfRoundCash()
    {
        Log.Debug("AfterEndOfRoundCash");
        InGame.instance.GetPlayerIndices().ForEachTrue(x => SalariedMonkeys.PaySalaries(x));
    }

    // Hooks for updating cash display.
    public static void OnSellTower(Tower tower) => SalariedMonkeys.OnSellTower(tower);

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
        // If the gamemode is chimps, increase round count.
        // Note: GameMode variable is not yet set, so we inspect the model list for expected name.
        foreach (var mod in mods)
        {
            if (mod.name != ModeType.CHIMPS) 
                continue;

            result.endRound += 40;
            break;
        }

        SalariedMonkeys.ConstructInGame(new TowerManager(BloonsApi.Instance, _modSettings), result);
    }
}