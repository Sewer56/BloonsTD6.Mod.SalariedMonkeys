using BloonsTD6.Mod.SalariedMonkeys.Interfaces;

namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

/// <summary>
/// Class that manages all the towers controlled by Monkeys for Hire.
/// </summary>
public class TowerManager
{
    /// <summary>
    /// The API implementation used to purchase/sell stuff in Bloons.
    /// </summary>
    public IBloonsApi BloonsApi { get; set; }

    /// <summary>
    /// Settings of the mod itself.
    /// </summary>
    public ModSettings Settings { get; set; }

    public TowerManager(IBloonsApi bloonsApi, ModSettings settings)
    {
        BloonsApi = bloonsApi;
        Settings = settings;
    }

    /// <summary>
    /// Returns the amount of available salary.
    /// </summary>
    /// <returns></returns>
    public double GetAvailableSalary(out double totalSalary)
    {
        totalSalary = GetTotalSalary();
        return BloonsApi.GetCash() - totalSalary;
    }

    /// <summary>
    /// Returns the amount of payable money to the monkeys.
    /// </summary>
    public double GetTotalSalary()
    {
        var towers = BloonsApi.GetTowers();
        var totalSalary = 0.0;

        foreach (var tower in towers)
            totalSalary += tower.GetSalary(Settings);

        return totalSalary;
    }

    /// <summary>
    /// Sells all towers, starting from cheapest tower until salary demand is met.
    /// </summary>
    /// <param name="requiredSalary">The required amount of money to be made.</param>
    /// <remarks>To be used in co-op!</remarks>
    /// <returns>Amount of new payable salary gained.</returns>
    public double SellTowers(float requiredSalary)
    {
        // Get towers in ascending order.
        var towers = BloonsApi.GetTowers();
        towers.Sort((a, b) => a.GetTotalCost().CompareTo(b.GetTotalCost()));

        // Current amount of cash gained.
        var salaryGained = 0.0;
        while (salaryGained < requiredSalary)
        {
            var tower = towers.FirstOrDefault();

            // We might not have any towers left to sell.
            if (tower == null)
                break;

            salaryGained += tower.GetSalary(Settings);
            tower.Sell();
            towers.Remove(tower);
        }

        return salaryGained;
    }
}