using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.Upgrade;
using BloonsTD6.Mod.SalariedMonkeys.Implementation;
using BTD_Mod_Helper.Api.ModOptions;
using HarmonyLib;
using UnhollowerBaseLib;

[assembly: MelonInfo(typeof(BloonsTD6.Mod.SalariedMonkeys.Mod), "Salaried Monkeys", "1.0.0", "Sewer56")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonsTD6.Mod.SalariedMonkeys;

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
    
    private static CashDisplay? _cashDisplay;
    private static ModSettings _modSettings = new ModSettings();
    
    public override void OnTitleScreen()
    {
        // Initialise Mod.
        CostPercentPerRound.OnValueChanged.Add(d => _modSettings.CostPercentPerRound = (float)(d / 100.0));
        SalariedMonkeys.Instance.Initialise(new TowerManager(BloonsApi.Instance, _modSettings));
    }

    public override void OnRoundEnd()
    {
        SalariedMonkeys.Instance.PaySalaries();
    }

    public override void OnUpdate()
    {
        // TODO: Why does catching exceptions not suppress them in log? Annoying.
        // Update cash display string every frame.
        try { _cashDisplay?.OnCashChanged(); }
        catch (Il2CppException e) { /* Suppress errors */ }
        catch (Exception e) { /* Suppress errors */ }
    }

    public override void OnMatchEnd()
    {
        // TODO: This is a hack! Find a better way to do this.
        _cashDisplay = null;
    }

    // Button to purchase tower on right side.
    public static void AfterTowerPurchaseButton_Update(TowerPurchaseButton instance)
    {
        if (instance.towerModel == null)
            return;
        
        var upgradeCost = _modSettings.CalculateCost(SalariedMonkeys.Instance.GetTowerInfo(instance.towerModel).TotalCost);
        if (instance.cost == upgradeCost)
            return;

        // This also disables purchasing if too low.
        instance.cost = upgradeCost;
        instance.UpdateTowerCost();
    }

    // Upgrade button inside left/right menu
    public static void AfterUpgradeButton_Update(UpgradeButton instance)
    {
        var upgrade = instance.upgrade;
        if (upgrade == null)
            return;

        var upgradeCost = _modSettings.CalculateCost(SalariedMonkeys.Instance.GetUpgradeCost(upgrade));
        instance.Cost.text = $"{upgradeCost:#####.#}";
    }

    // Selection menu on right/left hand side
    public static void AfterTowerSelectionMenu_OnUpdate(TowerSelectionMenu instance)
    {
        if (instance.sellText == null || instance.selectedTower == null)
            return;

        var tower = instance.selectedTower.tower;
        if (tower == null)
            return;

        var upgradeCost = _modSettings.CalculateCost(SalariedMonkeys.Instance.GetTowerInfo(tower).TotalCost);
        instance.sellText.text = $"{upgradeCost:#####.#}";
    }

    // Cash display on top of screen
    public static void AfterCashDisplay_OnCashChanged(CashDisplay instance)
    {
        _cashDisplay = instance;

        var text = instance.text;
        var rectTransform = text.rectTransform;
        rectTransform.offsetMax = new UnityEngine.Vector2(2000.0f, 50.0f);
        text.text += $" (-{SalariedMonkeys.Instance.TowerManager.GetTotalSalary():####0.#})";
    }

    // Upgrade menu.
    public static void AfterUpgradeDetails_UpdateSelected(SelectedUpgrade selectedUpgrade)
    {
        var upgradeDetails = selectedUpgrade.selectedDetails;
        if (upgradeDetails == null)
            return;

        if (upgradeDetails.upgrade == null)
            return;

        var upgradeCost = _modSettings.CalculateCost(SalariedMonkeys.Instance.GetUpgradeCost(upgradeDetails.upgrade));
        selectedUpgrade.unlockCost.text = $"{upgradeCost:#####.#}";
    }
}