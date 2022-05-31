using Assets.Scripts.Simulation.Towers.Behaviors;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using Vector3 = Assets.Scripts.Simulation.SMath.Vector3;

namespace BloonsTD6.Mod.SalariedMonkeys.Tests.Mocks;

internal class FakeBloonsApi : IBloonsApi
{
    public double Cash;
    public List<ISalariedTower> Towers;
    public bool AllowSelling;
    public int TowersSold;
    public bool IsRoundActiveValue;

    public FakeBloonsApi(double cash, List<ISalariedTower> towers)
    {
        Cash = cash;
        Towers = towers;
    }

    public int GetPlayerIndex() => 0;

    public double GetCash(int playerIndex) => Cash;

    public void AddCash(double amount, int playerIndex) => Cash += amount;

    public List<ISalariedTower> GetTowers(int playerIndex) => Towers;

    public bool? ToggleSelling(bool allowSelling)
    {
        var original = AllowSelling;
        AllowSelling = allowSelling;
        return original;
    }

    public bool IsRoundActive() => IsRoundActiveValue;

    public Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<DiscountZone>> GetDiscountInfo(Vector3 position, int path, int tier)
    {
        return new Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<DiscountZone>>();
    }
}