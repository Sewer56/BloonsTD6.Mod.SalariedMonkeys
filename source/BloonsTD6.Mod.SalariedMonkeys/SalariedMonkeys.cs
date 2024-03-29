﻿using Assets.Scripts.Models.Profile;
using Assets.Scripts.Simulation;
using Math = System.Math;

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

    /// <summary>
    /// Tracks removed towers for this game session.
    /// </summary>
    public RemovedTowerTracker RemovedTowerTracker { get; private set; } = new();

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
        RemovedTowerTracker = new RemovedTowerTracker();
        IsInitialized = true;
    }

    /// <summary>
    /// Called when the game restarts/is reset.
    /// </summary>
    public void OnGameRestart() => RemovedTowerTracker = new RemovedTowerTracker();

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
    public void PaySalaries(int playerIndex) => Api.AddCash(-TowerManager.GetTotalSalary(playerIndex, true), playerIndex);

    /// <summary>
    /// Sells towers if the player is in the negative.
    /// </summary>
    public void SellTowers(int playerIndex)
    {
        // Enable selling temporarily if necessary.
        var cash = Api.GetCash(playerIndex);
        if (cash >= 0)
            return;

        ForceFreeSelling = true;
        bool? originalSellState = null;

        try
        {
            originalSellState = Api.ToggleSelling(true);
            TowerManager.SellTowers(playerIndex, (float)Math.Abs(cash));
        }
        finally
        {
            ForceFreeSelling = false;
            if (originalSellState.HasValue)
                Api.ToggleSelling(originalSellState.Value);
        }
    }

    /// <summary>
    /// Returns true if salaries shouldn't be paid for this round.
    /// </summary>
    public bool ShouldSkipPaySalaries()
    {
        var isBoss = Api.IsBossEvent();
        if (!isBoss)
            return false;

        var round = Api.GetCurrentRound();
        return round is 40 or 60 or 80 or 100 or 120;
    }

    /// <summary>
    /// Gets the extra amount of rounds for a given difficulty.
    /// </summary>
    /// <param name="difficulty">Name of the difficulty.</param>
    public int GetExtraRoundCount(string difficulty)
    {
        if (difficulty == ModeType.CHIMPS)
            return 40;
        
        if (difficulty == ModeType.Impoppable)
            return 40;
        
        if (difficulty == ModeType.AlternateBloonsRounds)
            return 20;

        return 0;
    }

    /// <summary>
    /// Event handler for when the user sells a tower.
    /// </summary>
    /// <param name="tower">The tower to be sold.</param>
    /// <param name="increaseTowerWorth">True to increase tower worth by the sell amount.</param>
    public void OnSellTower(Tower tower, bool increaseTowerWorth = false)
    {
        if (ForceFreeSelling || Settings.SellPenalty == SellPenaltyKind.Free)
            return;

        if (Settings.SellPenalty == SellPenaltyKind.Always ||
            (Settings.SellPenalty == SellPenaltyKind.FreeBetweenRounds && Api.IsRoundActive()))
        {
            var sellAmount = this.CalculateSalaryWithDiscount(tower);
            if (increaseTowerWorth)
                tower.worth += sellAmount;

            Api.AddCash(-sellAmount, tower.GetOwnerZeroBased());
        }
    }

    /// <summary>
    /// Call this when a tower gets destroyed.
    /// </summary>
    public void OnTowerDestroyed(Tower tower) => RemovedTowerTracker.AddTower(tower);

    /// <summary>
    /// True if a game loss should be triggered.
    /// </summary>
    public bool ShouldTriggerLoss()
    {
        var totalCash = 0.0;
        Api.GetAvailablePlayers().ForEachTrue(x => totalCash += Api.GetCash(x));
        return (Api.IsLastRound() && totalCash < 0.0);
    }

    /// <summary>
    /// Call this when a save file is created.
    /// </summary>
    public void OnCreateSave(MapSaveDataModel mapData) => mapData.metaData[RemovedTowerTracker.MetadataEntryName] = RemovedTowerTracker.Serialize();

    /// <summary>
    /// Call this when a save file is loaded.
    /// </summary>
    public void OnLoadSave(MapSaveDataModel mapData, Simulation sim)
    {
        if (Il2CppDictionaryExtensions.TryGetValue(mapData.metaData, RemovedTowerTracker.MetadataEntryName, out var value))
            RemovedTowerTracker = RemovedTowerTracker.Deserialize(value);
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
        return monkeys.Settings.CalculateCost(baseCost, monkeys.Api.IsFreeplay());
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

        return monkeys.Settings.CalculateCost(baseCost, monkeys.Api.IsFreeplay());
    }

    /// <summary>
    /// Calculates the salary per round of a given upgrade.
    /// </summary>
    /// <param name="monkeys"></param>
    /// <param name="upgrade">The model representing the individual upgrade.</param>
    public static float CalculateSalary(this SalariedMonkeys monkeys, UpgradeModel upgrade)
    {
        var baseCost = monkeys.GetRawUpgradeCost(upgrade);
        return monkeys.Settings.CalculateCost(baseCost, monkeys.Api.IsFreeplay());
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