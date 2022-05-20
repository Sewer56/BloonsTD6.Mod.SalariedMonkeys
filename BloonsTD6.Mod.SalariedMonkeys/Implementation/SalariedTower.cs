using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity.UI_New.InGame;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BTD_Mod_Helper.Extensions;

namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

internal class SalariedTower : ISalariedTower
{
    public Tower BaseTower { get; }
    public float TotalCost { get; }

    public SalariedTower(Tower baseTower, float totalCost)
    {
        BaseTower = baseTower;
        TotalCost = totalCost;
    }

    public float GetTotalCost() => TotalCost;

    public void Sell() => InGame.instance.SellTower(BaseTower);
}