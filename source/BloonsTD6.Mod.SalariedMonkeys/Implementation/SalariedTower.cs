using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity.UI_New.InGame;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BloonsTD6.Mod.SalariedMonkeys.Structures;
using BTD_Mod_Helper.Extensions;

namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

internal class SalariedTower : ISalariedTower
{
    public Tower BaseTower { get; }
    public TowerInfo TowerInfo { get; }

    public SalariedTower(Tower baseTower, TowerInfo towerInfo)
    {
        BaseTower = baseTower;
        TowerInfo = towerInfo;
    }

    public TowerInfo GetTowerInfo() => TowerInfo;

    public float GetTotalCost() => TowerInfo.CalculateCostWithDiscounts(BloonsApi.Instance, BaseTower);

    public void Sell() => InGame.instance.SellTower(BaseTower);
}