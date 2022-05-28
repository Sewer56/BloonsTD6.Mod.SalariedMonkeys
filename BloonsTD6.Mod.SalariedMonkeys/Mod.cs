using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Simulation.Objects;
using Assets.Scripts.Simulation.SMath;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.Upgrade;
using BloonsTD6.Mod.SalariedMonkeys.Implementation;
using BloonsTD6.Mod.SalariedMonkeys.Utilities;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
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

    private static readonly ModSettingBool DisableIncome = new ModSettingBool(true)
    {
        displayName = "Disable Income",
    };

    private static readonly ModSettingEnum<SellPenaltyKind> SellPenalty = new ModSettingEnum<SellPenaltyKind>(SellPenaltyKind.FreeBetweenRounds)
    {
        displayName = "Selling Mode",
    };

    private static SalariedMonkeys SalariedMonkeys => SalariedMonkeys.Instance;
    private static CashDisplay? _cashDisplay;
    private static ModSettings _modSettings = new ModSettings();
    private static CachedStringFormatter _cachedStringFormatter = new CachedStringFormatter();
    private static bool _invalidateCashDisplay = false;

    /*
     * TODO: Co-Op
     * TODO: Geraldo
     * TODO: Tower Discounts / Discount Villages
     */

    public override void OnTitleScreen()
    {
        // Initialise Mod.
        OnPreferencesLoaded();
        CostPercentPerRound.OnValueChanged.Add(SetCostPerRoundFromSlider);
        DisableIncome.OnValueChanged.Add(SetDisableIncome);
        SellPenalty.OnValueChanged.Add(SetSellPenaltyType);
        ApplySettings();
    }

    public override void OnRoundEnd()
    {
        SalariedMonkeys.PaySalaries();

#if DEBUG
        var towers = SalariedMonkeys.Api.GetTowers();
        var totalCost = towers.Sum(x => x.GetTotalCost());
        MelonLogger.Msg($"Total Tower Cost: {totalCost}");
#endif
    }

    public override void OnMatchEnd()
    {
        // TODO: This is a hack! Find a better way to do this.
        _cashDisplay = null;
        _cachedStringFormatter.Clear();
        SalariedMonkeys.DeInitialize();
        GC.Collect();
    }

    // Hooks for updating cash display.
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse) => _invalidateCashDisplay = true;

    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel) => _invalidateCashDisplay = true;

    public override void OnTowerDestroyed(Tower tower)
    {
        // Make the user pay for the tower for the round if selling before round end.
        SalariedMonkeys.OnSellTower(tower);
        _invalidateCashDisplay = true;
    }

    // Executed every frame.
    public override void OnUpdate()
    {
        /*
            Limit updates of certain UI elements to 1 per frame.

            This is also a questionable bugfix to salary display in cash counter 
            not updating correctly when buying an upgrade that applies a discount.
        */

        if (!_invalidateCashDisplay)
            return;

        _cashDisplay?.OnCashChanged();
        _invalidateCashDisplay = false;
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

        var upgradeCost = SalariedMonkeys.CalculateSalaryWithDiscount(upgrade, instance.tts.tower);
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

        var upgradeCost = SalariedMonkeys.CalculateSalaryWithDiscount(tower);
        instance.sellText.text = _cachedStringFormatter.GetUpgradeCostWithDollar(upgradeCost);
    }

    // Upgrade menu. Can be accessed from main menu.
    public static void AfterUpgradeDetails_UpdateSelected(SelectedUpgrade selectedUpgrade)
    {
        // Don't allow access from main menu. This class is only inited in game.
        if (!SalariedMonkeys.IsInitialized)
            return;

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

    // Applying Settings Menu Settings.
    private void ApplySettings()
    {
        SetCostPerRoundFromSlider((double)CostPercentPerRound.GetValue());
        SetDisableIncome(DisableIncome);
        SetSellPenaltyType(SellPenalty);
    }

    private void SetSellPenaltyType(SellPenaltyKind value) => _modSettings.SellPenalty = value;

    private void SetDisableIncome(bool value) => _modSettings.DisableIncome = value;

    private void SetCostPerRoundFromSlider(double d) => _modSettings.CostPercentPerRound = (float)(d / 100.0);
}