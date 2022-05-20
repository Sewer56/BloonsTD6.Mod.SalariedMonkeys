using Assets.Scripts.Simulation.Towers;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;

namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

internal class SalariedTower : ISalariedTower
{
    public Tower BaseTower { get; private set; }
    public float TotalCost { get; private set; }

    public SalariedTower(Tower baseTower, float totalCost)
    {
        BaseTower = baseTower;
        TotalCost = totalCost;
    }

    public float GetTotalCost() => TotalCost;
    public void Sell() => BloonsApi.Instance.SellTower(this);
}