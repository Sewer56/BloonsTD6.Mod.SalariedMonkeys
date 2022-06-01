using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BloonsTD6.Mod.SalariedMonkeys.Structures;
using BloonsTD6.Mod.SalariedMonkeys.Utilities;
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
    public bool ForceFreeSelling { get; private set; } = false;

    /// <summary>
    /// True if this class is initialized, else false.
    /// </summary>
    public bool IsInitialized { get; private set; } = false;

    public ModClientSettings Settings => TowerManager.Settings;
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
        IsInitialized = true;
    }

    /// <summary>
    /// Marks the instance as uninitialized.
    /// </summary>
    public void DeInitialize() => IsInitialized = false;

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
    /// Does not include discounts.
    /// </summary>
    /// <param name="upgrade">The upgrade in question.</param>
    public float GetRawUpgradeCost(UpgradeModel upgrade)
    {
        if (_upgradeToCost.TryGetValue(upgrade.name, out var value))
            return value;

        return default;
    }
    
    /// <summary>
    /// Pays the salaries to all the monkeys.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void PaySalaries(int playerIndex) => Api.AddCash(-TowerManager.GetTotalSalary(playerIndex), playerIndex);

    /// <summary>
    /// Sells towers if the player is in the negative.
    /// </summary>
    public void SellTowers(int playerIndex)
    {
        // Enable selling temporarily if necessary.
        ForceFreeSelling = true;
        bool? originalSellState = null;

        try
        {
            originalSellState = Api.ToggleSelling(true);
            var available = TowerManager.GetAvailableSalary(playerIndex, out double totalSalary);
            if (available < 0)
                TowerManager.SellTowers(playerIndex, (float)Math.Abs(available));
        }
        finally
        {
            ForceFreeSelling = false;
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
        if (ForceFreeSelling || Settings.SellPenalty == SellPenaltyKind.Free)
            return;

        if (Settings.SellPenalty == SellPenaltyKind.Always ||
            (Settings.SellPenalty == SellPenaltyKind.FreeBetweenRounds && Api.IsRoundActive()))
        {
            Api.AddCash(-this.CalculateSalaryWithDiscount(tower), tower.GetOwnerZeroBased());
        }
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
        return monkeys.Settings.CalculateCost(baseCost);
    }

    /// <summary>
    /// Calculates the salary per round of a given tower.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="model">The model representing the individual tower.</param>
    public static float CalculateSalaryWithDiscount(this SalariedMonkeys monkeys, Tower model)
    {
        var baseCost = monkeys.GetTowerInfo(model)
                              .CalculateCostWithDiscounts(monkeys.Api, model);

        return monkeys.Settings.CalculateCost(baseCost);
    }

    /// <summary>
    /// Calculates the salary per round of a given upgrade.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="upgrade">The model representing the individual upgrade.</param>
    public static float CalculateSalary(this SalariedMonkeys monkeys, UpgradeModel upgrade)
    {
        var baseCost = monkeys.GetRawUpgradeCost(upgrade);
        return monkeys.Settings.CalculateCost(baseCost);
    }

    /// <summary>
    /// Calculates the salary per round of a given upgrade.
    /// Includes discount if possible.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="upgrade">The model representing the individual upgrade.</param>
    /// <param name="tower">The tower for which to calculate the discounts.</param>
    public static float CalculateSalaryWithDiscount(this SalariedMonkeys monkeys, UpgradeModel upgrade, Tower tower)
    {
        var baseSalary = monkeys.CalculateSalary(upgrade);
        var discounts = monkeys.Api.GetDiscountInfo(BloonsExtensions.ToVector3(tower.Position), upgrade.path, upgrade.tier);
        return baseSalary * discounts.CalcTotalDiscountMultiplier();
    }

    /// <summary>
    /// Constructs the mod from the current game instance.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="towerManager">The tower manager to use for the class.</param>
    /// <param name="model">The model to use for the current game.</param>
    public static void ConstructInGame(this SalariedMonkeys monkeys, ITowerManager towerManager, GameModel model)
    {
        var towerToCost = new Dictionary<string, TowerInfo>();
        var upgradeToCost = new Dictionary<string, float>();

        // Set tower costs.
        foreach (var tower in model.towers)
        {
            var id = tower.GetTowerId();
            var towerCost = tower.cost;
            var upgradeCost = 0.0f;
            var upgrades = new UpgradeModel[tower.appliedUpgrades.Count];

            for (var x = 0; x < tower.appliedUpgrades.Count; x++)
            {
                var upgrade = tower.appliedUpgrades[x];
                var upgradeModel = Game.instance.model.GetUpgrade(upgrade);

                upgrades[x] = upgradeModel;
                upgradeCost += upgradeModel.cost;
            }

            towerToCost[id] = new TowerInfo()
            {
                TowerCost = towerCost,
                UpgradeCost = upgradeCost,
                Upgrades = upgrades
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