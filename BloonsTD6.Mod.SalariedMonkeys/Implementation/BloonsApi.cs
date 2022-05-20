using Assets.Scripts.Unity.UI_New.InGame;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BTD_Mod_Helper.Extensions;

namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

internal class BloonsApi : IBloonsApi
{
    public static readonly BloonsApi Instance = new BloonsApi();

    public double GetCash() => InGame.instance.GetCash();

    public void AddCash(double amount) => InGame.instance.AddCash(amount);

    public List<ISalariedTower> GetTowers()
    {
        // We use this specific method because it doesn't include towers that have been destroyed.
        var towers = InGame.instance.GetAllTowerToSim();
        var list   = new List<ISalariedTower>();
        
        foreach (var tower in towers)
        {
            list.Add(new SalariedTower(tower.tower, SalariedMonkeys.Instance.GetTowerInfo(tower.tower).TotalCost));
        }

        return list;
    }

    public void SellTower(ISalariedTower tower)
    {
        var salaried = (SalariedTower)(tower);
        InGame.instance.SellTower(salaried.BaseTower);
    }
}