using BloonsTD6.Mod.SalariedMonkeys.Interfaces;

namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

public class TowerManager : ITowerManager
{
    /// <inheritdoc/>
    public IBloonsApi BloonsApi { get; set; }

    /// <inheritdoc/>
    public ModSettings Settings { get; set; }

    public TowerManager(IBloonsApi bloonsApi, ModSettings settings)
    {
        BloonsApi = bloonsApi;
        Settings = settings;
    }

    /// <inheritdoc/>
    public double GetAvailableSalary(out double totalSalary)
    {
        totalSalary = GetTotalSalary();
        return BloonsApi.GetCash() - totalSalary;
    }

    /// <inheritdoc/>
    public double GetTotalSalary()
    {
        var towers = BloonsApi.GetTowers();
        var totalSalary = 0.0;

        foreach (var tower in towers)
            totalSalary += tower.GetSalary(Settings);

        return totalSalary;
    }


    /// <inheritdoc/>
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