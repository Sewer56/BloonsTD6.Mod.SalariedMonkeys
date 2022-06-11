// ReSharper disable InconsistentNaming

using System.Runtime.InteropServices;
using Assets.Scripts.Models.Profile;
using Assets.Scripts.Simulation;
using Assets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Simulation.Utils;
using Assets.Scripts.Unity.Bridge;
using BloonsTD6.Mod.SalariedMonkeys.Utilities.Hooks;
using TowerManager = Assets.Scripts.Simulation.Towers.TowerManager;

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

[HarmonyPatch(typeof(TowerManager), nameof(TowerManager.SellTower))]
public class SellTower_Run
{
    [HarmonyPrefix]
    public static void Prefix(TowerManager __instance, ref Tower tower) => Mod.OnSellTower(tower);
}

[HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.Win))]
public class WinAction_Run
{
    [HarmonyPrefix]
    public static bool Prefix() => Mod.BeforeRunWinAction();
}

[HarmonyPatch(typeof(InGame), nameof(InGame.RoundEnd))]
internal class InGame_RoundEnd
{
    [HarmonyPrefix]
    public static void Prefix() => Mod.BeforeRoundEnd();
}

[HarmonyPatch(typeof(BloodSacrifice), nameof(BloodSacrifice.Activate))]
public class BloodSacrifice_Activate
{
    [HarmonyPrefix]
    public static void Prefix(BloodSacrifice __instance) => Mod.BeforeActivateBloodSacrifice(__instance);
}

[HarmonyPatch(typeof(ParagonTower), nameof(ParagonTower.StartSacrifice))]
public class ParagonTower_StartSacrifice
{
    [HarmonyPrefix]
    public static void Prefix(ParagonTower __instance) => Mod.BeforeParagonSacrifice(__instance);

    [HarmonyPostfix]
    public static void PostFix(ParagonTower __instance) => Mod.AfterParagonSacrifice(__instance);
}

[HarmonyPatch(typeof(MapSaveLoader), nameof(MapSaveLoader.LoadMapSaveData))]
public class MapSaveLoader_LoadMapSaveData
{
    [HarmonyPostfix]
    public static void PostFix(ref MapSaveDataModel mapData, ref Simulation sim) => Mod.AfterLoadMapSave(mapData, sim);
}

public class NativeHooks
{
    /// <summary>
    /// Uses IntPtr to ignore the generic type! Beware!!
    /// </summary>
    public static NativeHook<CreateMapSaveData_Fn> LoadFromFileStorageHook = new Il2CppNativeHookable<CreateMapSaveData_Fn>(AccessTools.Method(typeof(MapSaveLoader), nameof(MapSaveLoader.CreateMapSaveData))).Hook(LoadFromFileStorageImpl).Activate();

    private static IntPtr LoadFromFileStorageImpl(IntPtr mapsavedataid, int gameid, IntPtr simulation, IntPtr gamesettings, IntPtr heroes, int round, IntPtr dailychallengeeventid)
    {
        var result = LoadFromFileStorageHook.OriginalFunction(mapsavedataid, gameid, simulation, gamesettings, heroes, round, dailychallengeeventid);
        Mod.AfterCreateMapSave(new MapSaveDataModel(result));
        return result;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr CreateMapSaveData_Fn
    (
        IntPtr mapSaveDataID,
        int gameId,
        IntPtr simulation,
        IntPtr gameSettings,
        IntPtr heroes,
        int round,
        IntPtr dailyChallengeEventId
    );

    public static void Init() { /* Dummy for static initialisation */ }
}
