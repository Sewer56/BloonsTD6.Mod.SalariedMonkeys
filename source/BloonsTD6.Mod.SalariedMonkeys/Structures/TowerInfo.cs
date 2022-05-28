using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Simulation.Towers;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BloonsTD6.Mod.SalariedMonkeys.Utilities;

namespace BloonsTD6.Mod.SalariedMonkeys.Structures;

public class TowerInfo
{
    /// <summary>
    /// Total cost of the tower, including upgrades.
    /// Does not include discounts.
    /// <see cref="CalculateCostWithDiscounts"/> to get total cost with discount.
    /// </summary>
    public float TotalCost => TowerCost + UpgradeCost;

    /// <summary>
    /// Cost of only the tower itself.
    /// Excluding discounts.
    /// </summary>
    public float TowerCost;

    /// <summary>
    /// Cost of only upgrades.
    /// Excluding discounts.
    /// </summary>
    public float UpgradeCost;

    /// <summary>
    /// Array of all upgrades that apply to this tower.
    /// </summary>
    public UpgradeModel[] Upgrades = Array.Empty<UpgradeModel>();

    /// <summary>
    /// Calculates the total cost of the tower with discounts.
    /// </summary>
    /// <returns>The total cost of the tower.</returns>
    public float CalculateCostWithDiscounts(IBloonsApi api, Tower baseTower)
    {
        var monkeys = SalariedMonkeys.Instance;
        var towerPos = BloonsExtensions.ToVector3(baseTower.Position);

        // Calculate Upgrade Costs
        var totalUpgradeCost = 0.0f;
        foreach (var upgrade in Upgrades)
        {
            var baseSalary = monkeys.GetRawUpgradeCost(upgrade);
            var discounts  = monkeys.Api.GetDiscountInfo(towerPos, upgrade.path, upgrade.tier);
            totalUpgradeCost += baseSalary * discounts.CalcTotalDiscountMultiplier();
        }

        // Calculate tower cost
        var towerMultiplier = api.GetDiscountInfo(towerPos, 0, 0).CalcTotalDiscountMultiplier();
        var totalTowerCost = TowerCost * towerMultiplier;

        return totalTowerCost + totalUpgradeCost;
    }
}