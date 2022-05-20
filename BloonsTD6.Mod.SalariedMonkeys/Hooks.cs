using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.Upgrade;
using HarmonyLib;
// ReSharper disable InconsistentNaming

namespace BloonsTD6.Mod.SalariedMonkeys;

[HarmonyPatch(typeof(TowerPurchaseButton), nameof(TowerPurchaseButton.Update))]
public class TowerPurchaseButtonHooks
{
    [HarmonyPostfix]
    public static void PostFix(TowerPurchaseButton __instance) => Mod.AfterTowerPurchaseButton_Update(__instance);
}

[HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.UpdateCostVisuals))]
public class UpdateCostOverride
{
    [HarmonyPostfix]
    public static void PostFix(UpgradeButton __instance) => Mod.AfterUpgradeButton_Update(__instance);
}

[HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.OnUpdate))]
public class TowerSelectionMenuHooks
{
    [HarmonyPostfix]
    public static void PostFix(TowerSelectionMenu __instance) => Mod.AfterTowerSelectionMenu_OnUpdate(__instance);
}

[HarmonyPatch(typeof(CashDisplay), nameof(CashDisplay.OnCashChanged))]
public class CashDisplayHooks
{
    [HarmonyPostfix]
    public static void Postfix(CashDisplay __instance) => Mod.AfterCashDisplay_OnCashChanged(__instance);
}

[HarmonyPatch(typeof(SelectedUpgrade), nameof(UpgradeDetails.Update))]
public class UpgradeDetailsHooks
{
    [HarmonyPostfix]
    public static void Postfix(SelectedUpgrade __instance) => Mod.AfterUpgradeDetails_UpdateSelected(__instance);
}
