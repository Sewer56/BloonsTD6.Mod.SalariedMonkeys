using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BloonsTD6.Mod.SalariedMonkeys.Structures;
using BTD_Mod_Helper.Extensions;

namespace BloonsTD6.Mod.SalariedMonkeys;

public class SalariedMonkeys
{
    /// <summary>
    /// Singleton instance of this class.
    /// </summary>
    public static readonly SalariedMonkeys Instance = new SalariedMonkeys();

    /// <summary>
    /// The tower manager associated with this mod instance.
    /// </summary>
    public ITowerManager TowerManager { get; private set; } = null!;

    /// <summary>
    /// This is set to true when salaries are currently being paid.
    /// </summary>
    public bool IsPayingSalaries { get; private set; } = false;

    public ModSettings Settings => TowerManager.Settings;
    public IBloonsApi Api => TowerManager.BloonsApi;

    private Dictionary<string, float> _upgradeToCost = null!;
    private Dictionary<string, TowerInfo> _towerToCost = null!;

    /// <summary>
    /// Constructs this instance of the class.
    /// </summary>
    /// <param name="towerManager">The tower manager to use for the class.</param>
    /// <param name="upgradeToCost">Maps all tower upgrade names to corresponding costs.</param>
    /// <param name="towerToCost">Maps all corresponding to costs.</param>
    public void Construct(ITowerManager towerManager, Dictionary<string, float> upgradeToCost, Dictionary<string, TowerInfo> towerToCost)
    {
        TowerManager  = towerManager;
        _upgradeToCost = upgradeToCost;
        _towerToCost   = towerToCost;
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
        IsPayingSalaries = true;
        var originalSellState = Api.ToggleSelling(true);

        try
        {
            var available = TowerManager.GetAvailableSalary(out double totalSalary);
            if (available < 0)
                TowerManager.SellTowers((float)Math.Abs(available));

            Api.AddCash(-totalSalary);
        }
        finally
        {
            IsPayingSalaries = false;
            if (originalSellState.HasValue)
                Api.ToggleSelling(originalSellState.Value);
        }
    }

    /// <summary>
    /// Event handler for when the user sells a tower.
    /// </summary>
    /// <param name="tower">The tower to be sold.</param>
    public void OnSellTower(Tower tower)
    {
        if (IsPayingSalaries || Settings.SellPenalty == SellPenaltyKind.Free)
            return;
        
        if (Settings.SellPenalty == SellPenaltyKind.Always || 
            (Settings.SellPenalty == SellPenaltyKind.FreeBetweenRounds && Api.IsRoundActive()))
            Api.AddCash(-this.CalculateSalary(tower.towerModel));
    }
}

public static class SalariedMonkeysExtensions
{
    /// <summary>
    /// Calculates the salary per round of a given tower.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="model">The model representing the individual tower.</param>
    public static float CalculateSalary(this SalariedMonkeys monkeys, TowerModel model)
    {
        var baseCost = monkeys.GetTowerInfo(model).TotalCost;
        var multiplier = monkeys.Api.GetDifficultyCostMultiplier();
        return monkeys.Settings.CalculateCost(baseCost, multiplier);
    }

    /// <summary>
    /// Calculates the salary per round of a given upgrade.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="upgrade">The model representing the individual upgrade.</param>
    public static float CalculateSalary(this SalariedMonkeys monkeys, UpgradeModel upgrade)
    {
        var baseCost = monkeys.GetUpgradeCost(upgrade);
        var multiplier = monkeys.Api.GetDifficultyCostMultiplier();
        return monkeys.Settings.CalculateCost(baseCost, multiplier);
    }

    /// <summary>
    /// Constructs the mod from the current game instance.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="towerManager">The tower manager to use for the class.</param>
    public static void ConstructInGame(this SalariedMonkeys monkeys, ITowerManager towerManager)
    {
        var model = Game.instance.model;
        var towerToCost = new Dictionary<string, TowerInfo>();
        var upgradeToCost = new Dictionary<string, float>();

        // Set tower costs.
        foreach (var tower in model.towers)
        {
            var id = tower.GetTowerId();
            var towerCost = tower.cost;
            var upgradeCost = 0.0f;

            foreach (var upgrade in tower.appliedUpgrades)
            {
                var upgradeModel = Game.instance.model.GetUpgrade(upgrade);
                upgradeCost += upgradeModel.cost;
            }

            towerToCost[id] = new TowerInfo()
            {
                TowerCost = towerCost,
                UpgradeCost = upgradeCost
            };

            tower.cost = 0;
        }

        // Set all upgrades free.
        foreach (var upgrade in model.upgrades)
        {
            upgradeToCost[upgrade.name] = upgrade.cost;
            upgrade.cost = 0;
        }

        // Construct instance.
        monkeys.Construct(towerManager, upgradeToCost, towerToCost);
    }
}