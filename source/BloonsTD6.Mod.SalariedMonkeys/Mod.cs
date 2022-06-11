using System.Runtime.InteropServices;

[assembly: MelonInfo(typeof(BloonsTD6.Mod.SalariedMonkeys.Mod), "Salaried Monkeys", "1.0.0", "Sewer56")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to BTD6.
/// This class handles the bootstrapping part of the mod.
/// As such, it is not testable without going ingame.
/// </summary>
[ExcludeFromCodeCoverage] // game specific code
public partial class Mod : BloonsTD6Mod
{
    // Github API URL used to check if this mod is up to date. For example:
    public override string GithubReleaseURL => "https://api.github.com/repos/Sewer56/BloonsTD6.Mod.SalariedMonkeys/releases";

    // Shared parts between partial classes.
    private static IBloonsApi Api => SalariedMonkeys.Api;
    private static SalariedMonkeys SalariedMonkeys => SalariedMonkeys.Instance;
    private static ModClientSettings _modSettings = new ModClientSettings();
    private static int _invalidateCashDisplayTimer = 1;
    
    public override void OnTitleScreen() => Initialize_Settings();

    public override void OnMatchEnd()
    {
        OnMatchEnd_UI();
        OnMatchEnd_Core();
        GC.Collect();
    }

    // Sell towers when the round ends.
    public override void OnRoundEnd()
    {
        Log.Debug("OnRoundEnd");
        OnRoundEnd_Core();
        OnRoundEnd_UI();

#if DEBUG
        var towers = SalariedMonkeys.Api.GetTowers(0);
        var totalCost = towers.Sum(x => x.GetTotalCost());
        MelonLogger.Msg($"Total Tower Cost: {totalCost}");
#endif
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        Log.Debug("TowerCreated");
        OnTowerCreated_UI(tower, target, modelToUse);
    }

    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        Log.Debug("TowerUpgraded");
        OnTowerUpgraded_UI(tower, upgradeName, newBaseTowerModel);
    }

    public override void OnTowerDestroyed(Tower tower)
    {
        Log.Debug($"TowerDestroyed, Worth {tower.worth}");
        OnTowerDestroyed_UI(tower);
        OnTowerDestroyed_Core(tower);
    }

    // Executed every frame.
    public override void OnApplicationStart() => NativeHooks.Init();

    public override void OnUpdate() => OnUpdate_UI();
}