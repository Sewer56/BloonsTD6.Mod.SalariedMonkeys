// ReSharper disable InconsistentNaming

using Assets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using NinjaKiwi.LiNK.Lobbies;
using NinjaKiwi.NKMulti;
using NKMultiConnection = NinjaKiwi.NKMulti.NKMultiConnection;
using Task = Il2CppSystem.Threading.Tasks.Task;

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

[HarmonyPatch(typeof(GameModel), nameof(GameModel.CreateModded), typeof(GameModel), typeof(Il2CppSystem.Collections.Generic.List<ModModel>))]
public class GameModel_CreateModded
{
    [HarmonyPrefix]
    public static void DisableIncome(GameModel result, Il2CppSystem.Collections.Generic.List<ModModel> mods) => Mod.OnCreateModded(result, mods);

    [HarmonyPostfix]
    public static void CreateSalariedMonkeys(GameModel result, Il2CppSystem.Collections.Generic.List<ModModel> mods) => Mod.AfterCreateModded(result, mods);
}

[HarmonyPatch(typeof(SelectedUpgrade), nameof(UpgradeDetails.SetUpgrade))]
public class SelectedUpgrade_SetUpgrade
{
    [HarmonyPostfix]
    public static void Postfix(SelectedUpgrade __instance) => Mod.AfterUpgradeDetails_UpdateSelected(__instance);
}

[HarmonyPatch(typeof(BonusCashPerRound), nameof(BonusCashPerRound.OnRoundEnd))]
public class BonusCashPerRound_OnRoundEnd
{
    [HarmonyPostfix]
    public static void Postfix() => Mod.AfterAddEndOfRoundCash();
}

[HarmonyPatch(typeof(BloodSacrifice), nameof(BloodSacrifice.Activate))]
public class BloodSacrifice_Activate
{
    [HarmonyPrefix]
    public static void Prefix(BloodSacrifice __instance) => Mod.OnAdoraBloodSacrifice(__instance);

    [HarmonyPostfix]
    public static void Postfix(BloodSacrifice __instance) => Mod.AfterAdoraBloodSacrifice(__instance);
}

[HarmonyPatch(typeof(ParagonTower), nameof(ParagonTower.GetTowerInvestment))]
public class ParagonTower_StartSacrifice
{
    [HarmonyPrefix]
    public static void Prefix(ParagonTower __instance, ref Tower towerToUse) => Mod.OnParagonGetTowerInvestment(__instance, towerToUse);

    [HarmonyPostfix]
    public static void Postfix(ParagonTower __instance, ref Tower towerToUse) => Mod.AfterParagonGetTowerInvestment(__instance, towerToUse);
}