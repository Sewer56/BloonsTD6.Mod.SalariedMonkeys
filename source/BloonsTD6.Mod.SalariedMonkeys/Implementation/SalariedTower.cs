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

    public void IncreaseWorth(float amount) => BaseTower.worth += amount;
}