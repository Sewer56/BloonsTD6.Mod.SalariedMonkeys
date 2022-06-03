namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to performing the UI hooks.
/// </summary>
public partial class Mod
{
    private void OnMatchEnd_UI()
    {
        _cashDisplay = null;
        _cachedStringFormatter.Clear();
    }

    // Events running every frame corresponding to UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnUpdate_UI()
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
        if (!_modSettings.ShowSalaryInUI)
            return;

        var text = instance.text;
        if (text == null)
            return;

        var rectTransform = text.rectTransform;
        rectTransform.offsetMax = new UnityEngine.Vector2(2000.0f, rectTransform.offsetMax.y); // Extend max text width
        text.text += _cachedStringFormatter.GetSalary((float)SalariedMonkeys.TowerManager.GetTotalSalary(Api.GetPlayerIndex()));
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
        if (SalariedMonkeys.Api.GetCash(Api.GetPlayerIndex()) < upgradeCost)
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

        // Geraldo item hotfix until I decide if to give him special treatment or not.
        // Unknown items have a cost of 0.
        if (upgradeCost == 0)
            return;

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
        if (!_modSettings.ShowSalaryInUI)
            return;

        var pos = loanDisplay.transform.position;
        pos.x += 100;
        loanDisplay.transform.position = pos;
    }
}