using Assets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using TowerManager = BloonsTD6.Mod.SalariedMonkeys.Implementation.TowerManager;

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to the mod's core functionality.
/// </summary>
public partial class Mod
{
    private void OnMatchEnd_Core() => SalariedMonkeys.DeInitialize();

    // Handle lose condition on round end.
    private static bool _isLosing = false;

    public static bool BeforeRunWinAction()
    {
        if (!_isLosing)
            return true;

        _isLosing = false;
        return false; // Skip original
    }

    public static void BeforeRoundEnd()
    {
        var instance = InGame.instance;
        var totalCash = 0.0;
        
        InGame.instance.GetPlayerIndices().ForEachTrue(x => totalCash += Api.GetCash(x));
        if (instance.IsLastRound() && totalCash < 0.0)
        {
            // Prevent win from triggering
            instance.GetUnityToSimulation().Lose();
            _isLosing = true;
        }
    }

    // Sell towers when the round ends.
    public void OnRoundEnd_Core()
    {
        var inGame = InGame.instance;
        inGame.GetPlayerIndices().ForEachTrue(x => SalariedMonkeys.SellTowers(x));
        
        if (Api.GetCurrentRound() == 100)
            inGame.ShowRoundHint("You are entering Monkeys for Hire Freeplay Mode!\n" +
                                 "In this mode, tower salaries are severely reduced.\n" +
                                 "Good Luck!");
    }

    // We inject into the cash add function because Co-Op sends out a synchronization message
    // before the round end call, which makes syncing during round end more dangerous.
    public static void AfterAddEndOfRoundCash()
    {
        Log.Debug("AfterEndOfRoundCash");
        if (SalariedMonkeys.ShouldSkipPaySalaries())
        {
            Log.Debug("Skipping Salary Pay");
            return;
        }

        InGame.instance.GetPlayerIndices().ForEachTrue(x => SalariedMonkeys.PaySalaries(x));
    }

    // Blood sacrifice adora.
    public static void BeforeActivateBloodSacrifice(BloodSacrifice sacrifice)
    {
        var tower = InGame.instance.GetTowerManager().GetTowerById(sacrifice.selectedTowerId);
        if (tower != null)
            SalariedMonkeys.OnSellTower(tower, true);
    }

    // Performs cash changes for tower sell.
    public static void OnSellTower(Tower tower)
    {
        tower.worth = 0;
        SalariedMonkeys.OnSellTower(tower);
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
        // If the gamemode is chimps, increase round count.
        // Note: GameMode variable is not yet set, so we inspect the model list for expected name.
        foreach (var mod in mods)
        {
            if (mod.name == ModeType.CHIMPS) 
                result.endRound += 40;
            else if (mod.name == ModeType.Impoppable)
                result.endRound += 40;
            else if (mod.name == ModeType.AlternateBloonsRounds)
                result.endRound += 20;
        }

        SalariedMonkeys.ConstructInGame(new TowerManager(BloonsApi.Instance, _modSettings), result);
    }
}