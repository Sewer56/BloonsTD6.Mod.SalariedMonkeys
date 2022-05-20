using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BTD_Mod_Helper.Extensions;
using TowerManager = BloonsTD6.Mod.SalariedMonkeys.Implementation.TowerManager;

namespace BloonsTD6.Mod.SalariedMonkeys;

internal class SalariedMonkeys
{
    /// <summary>
    /// Singleton instance of this class.
    /// </summary>
    public static readonly SalariedMonkeys Instance = new SalariedMonkeys();
    public TowerManager TowerManager { get; private set; } = null!;

    private Dictionary<string, float> _upgradeToCost = new Dictionary<string, float>();
    private Dictionary<string, TowerInfo> _towerToCost = new Dictionary<string, TowerInfo>();

    private IBloonsApi Api => TowerManager.BloonsApi;

    /// <summary>
    /// Constructs a given instance of the mod.
    /// </summary>
    /// <param name="towerManager">The tower manager to initialise this instance with.</param>
    public void Initialise(TowerManager towerManager)
    {
        TowerManager = towerManager;

        // Set tower costs.
        var model = Game.instance.model;
        foreach (var tower in model.towers)
        {
            var id          = tower.GetTowerId();
            var towerCost   = tower.cost;
            var upgradeCost = 0.0f;
            
            foreach (var upgrade in tower.appliedUpgrades)
            {
                var upgradeModel = Game.instance.model.GetUpgrade(upgrade);
                upgradeCost += upgradeModel.cost;
            }

            _towerToCost[id] = new TowerInfo()
            {
                TowerCost   = towerCost,
                UpgradeCost = upgradeCost
            };
            
            tower.cost = 0;
        }

        // Set all upgrades free.
        foreach (var upgrade in Game.instance.model.upgrades)
        {
            _upgradeToCost[upgrade.name] = upgrade.cost;
            upgrade.cost = 0;
        }
    }

    /// <summary>
    /// Gets the upgrade info for a given tower.
    /// </summary>
    /// <param name="tower">The tower in question.</param>
    public TowerInfo GetTowerInfo(Tower tower) => GetTowerInfo(tower.towerModel);

    /// <summary>
    /// Gets the upgrade info for a given tower model.
    /// </summary>
    /// <param name="model">The tower model in question.</param>
    public TowerInfo GetTowerInfo(TowerModel model)
    {
        if (_towerToCost.TryGetValue(model.GetTowerId(), out var value))
            return value;

        return new TowerInfo();
    }

    /// <summary>
    /// Gets the upgrade cost for a given upgrade.
    /// </summary>
    /// <param name="upgrade">The upgrade in question.</param>
    public float GetUpgradeCost(UpgradeModel upgrade)
    {
        if (_upgradeToCost.TryGetValue(upgrade.name, out var value))
            return value;

        return default;
    }

    /// <summary>
    /// Pays the salaries to all the monkeys.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void PaySalaries()
    {
        // Enable selling temporarily if necessary.
        var model       = InGame.instance.GetGameModel();
        var sellEnabled = false;
        if (model != null)
        {
            sellEnabled = model.towerSellEnabled;
            model.towerSellEnabled = true;
        }

        try
        {
            var available = TowerManager.GetAvailableSalary(out double totalSalary);
            if (available < 0)
                TowerManager.SellTowers((float)Math.Abs(available));

            Api.AddCash(-totalSalary);
        }
        finally
        {
            if (model != null)
                model.towerSellEnabled = sellEnabled;
        }
    }
}

public class TowerInfo
{
    /// <summary>
    /// Total cost of the tower, including upgrades.
    /// </summary>
    public float TotalCost => TowerCost + UpgradeCost;

    /// <summary>
    /// Cost of only the tower itself.
    /// </summary>
    public float TowerCost;

    /// <summary>
    /// Cost of only upgrades.
    /// </summary>
    public float UpgradeCost;
}