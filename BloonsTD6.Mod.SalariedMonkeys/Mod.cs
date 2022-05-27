using Assets.Scripts.Models;
using Assets.Scripts.Models.Difficulty;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Simulation.Objects;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.Upgrade;
using BloonsTD6.Mod.SalariedMonkeys.Implementation;
using BloonsTD6.Mod.SalariedMonkeys.Utilities;
using BTD_Mod_Helper.Api.ModOptions;
using TowerManager = BloonsTD6.Mod.SalariedMonkeys.Implementation.TowerManager;

[assembly: MelonInfo(typeof(BloonsTD6.Mod.SalariedMonkeys.Mod), "Salaried Monkeys", "1.0.0", "Sewer56")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to BTD6.
/// As such, it is not testable without going ingame.
/// </summary>
public class Mod : BloonsTD6Mod
{
    // Github API URL used to check if this mod is up to date. For example:
    public override string GithubReleaseURL => "https://api.github.com/repos/Sewer56/BloonsTD6.Mod.SalariedMonkeys/releases";

    private static readonly ModSettingDouble CostPercentPerRound = new ModSettingDouble(5.00)
    {
        displayName = "Cost Percent Per Round",
        minValue = 0.0,
        maxValue = 100.0,
        isSlider = true
    };

    private static SalariedMonkeys SalariedMonkeys => SalariedMonkeys.Instance;
    private static CashDisplay? _cashDisplay;
    private static ModSettings _modSettings = new ModSettings();
    private static CachedStringFormatter _cachedStringFormatter = new CachedStringFormatter();

    public override void OnTitleScreen()
    {
        // Initialise Mod.
        OnPreferencesLoaded();
        CostPercentPerRound.OnValueChanged.Add(SetCostPerRoundFromSlider);
        SalariedMonkeys.ConstructInGame(new TowerManager(BloonsApi.Instance, _modSettings));
        ApplySettings();
    }

    public override void OnRoundEnd() => SalariedMonkeys.PaySalaries();

    public override void OnMatchEnd()
    {
        // TODO: This is a hack! Find a better way to do this.
        _cashDisplay = null;
        _cachedStringFormatter.Clear();
        BloonsApi.ResetCacheForNewMatch();
    }

    // Hooks for updating cash display.
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse) => _cashDisplay?.OnCashChanged();

    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel) => _cashDisplay?.OnCashChanged();

    public override void OnTowerDestroyed(Tower tower)
    {
        // Make the user pay for the tower for the round if selling before round end.
        SalariedMonkeys.OnSellTower(tower);
        _cashDisplay?.OnCashChanged();
    }

    // Cash display on top of screen
    public static void AfterCashDisplay_OnCashChanged(CashDisplay instance)
    {
        _cashDisplay = instance;

        var text = instance.text;
        var rectTransform = text.rectTransform;
        rectTransform.offsetMax = new UnityEngine.Vector2(2000.0f, rectTransform.offsetMax.y); // Extend max text width
        text.text += _cachedStringFormatter.GetSalary((float)SalariedMonkeys.TowerManager.GetTotalSalary());
    }

    // Button to purchase tower on right side.
    public static void AfterTowerPurchaseButton_Update(TowerPurchaseButton instance)
    {
        if (instance.towerModel == null)
            return;
        
        var upgradeCost = SalariedMonkeys.CalculateSalary(instance.towerModel);
        if (instance.cost == upgradeCost)
            return;

        // This also disables purchasing if too low.
        instance.cost = upgradeCost;
        instance.UpdateTowerCost();
    }

    // Upgrade button inside left/right menu
    public static void AfterUpgradeButton_UpdateCostVisuals(UpgradeButton instance)
    {
        // Show correct price.
        var upgrade = instance.upgrade;
        if (upgrade == null)
            return;

        var upgradeCost = SalariedMonkeys.CalculateSalary(upgrade);
        instance.Cost.text = _cachedStringFormatter.GetUpgradeCostWithDollar(upgradeCost);
        if (SalariedMonkeys.Api.GetCash() < upgradeCost)
            instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
    }

    // Selection menu on right/left hand side. Sell text.
    public static void AfterTowerSelectionMenu_OnUpdate(TowerSelectionMenu instance)
    {
        if (instance.sellText == null || instance.selectedTower == null)
            return;

        var tower = instance.selectedTower.tower;
        if (tower == null)
            return;

        var upgradeCost = SalariedMonkeys.CalculateSalary(tower.towerModel);
        instance.sellText.text = _cachedStringFormatter.GetUpgradeCostWithDollar(upgradeCost);
    }

    // Upgrade menu.
    public static void AfterUpgradeDetails_UpdateSelected(SelectedUpgrade selectedUpgrade)
    {
        var upgradeDetails = selectedUpgrade.selectedDetails;
        if (upgradeDetails == null || upgradeDetails.upgrade == null)
            return;

        var upgradeCost = SalariedMonkeys.CalculateSalary(upgradeDetails.upgrade);
        selectedUpgrade.unlockCost.text = _cachedStringFormatter.GetUpgradeCost(upgradeCost);
    }

    // IMF Loan Display, We need to move icon right so doesn't interfere with our cash counter.
    public static void LoanDisplay_Initialise(LoanDisplay loanDisplay)
    {
        var pos = loanDisplay.transform.position;
        pos.x += 100;
        loanDisplay.transform.position = pos;
    }

    // Disable income in other modes.
    public static void CreateModded(GameModel result, Il2CppSystem.Collections.Generic.List<ModModel> mods) => result.DisableIncome();

    private void ApplySettings() => SetCostPerRoundFromSlider((double)CostPercentPerRound.GetValue());
    private void SetCostPerRoundFromSlider(double d) => _modSettings.CostPercentPerRound = (float)(d / 100.0);
}