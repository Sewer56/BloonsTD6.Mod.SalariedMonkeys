using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.Upgrade;
using HarmonyLib;
// ReSharper disable InconsistentNaming

namespace BloonsTD6.Mod.SalariedMonkeys;

[HarmonyPatch(typeof(TowerPurchaseButton), nameof(TowerPurchaseButton.Update))]
public class TowerPurchaseButton_Update
{
    [HarmonyPostfix]
    public static void PostFix(TowerPurchaseButton __instance) => Mod.AfterTowerPurchaseButton_Update(__instance);
}

[HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.UpdateCostVisuals))]
public class UpgradeButton_UpdateCostVisuals
{
    [HarmonyPostfix]
    public static void PostFix(UpgradeButton __instance) => Mod.AfterUpgradeButton_UpdateCostVisuals(__instance);
}

[HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.UpdateTower))]
public class TowerSelectionMenu_UpdateTower
{
    [HarmonyPostfix]
    public static void PostFix(TowerSelectionMenu __instance) => Mod.AfterTowerSelectionMenu_OnUpdate(__instance);
}

[HarmonyPatch(typeof(CashDisplay), nameof(CashDisplay.OnCashChanged))]
public class CashDisplay_OnCashChanged
{
    [HarmonyPostfix]
    public static void Postfix(CashDisplay __instance) => Mod.AfterCashDisplay_OnCashChanged(__instance);
}

[HarmonyPatch(typeof(LoanDisplay), nameof(LoanDisplay.Initialise))]
public class LoanDisplay_Initialise
{
    [HarmonyPostfix]
    public static void Postfix(LoanDisplay __instance) => Mod.LoanDisplay_Initialise(__instance);
}

[HarmonyPatch(typeof(SelectedUpgrade), nameof(UpgradeDetails.SetUpgrade))]
public class SelectedUpgrade_SetUpgrade
{
    [HarmonyPostfix]
    public static void Postfix(SelectedUpgrade __instance) => Mod.AfterUpgradeDetails_UpdateSelected(__instance);
}
