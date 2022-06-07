namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

public class TowerManager : ITowerManager
{
    /// <inheritdoc/>
    public IBloonsApi BloonsApi { get; set; }

    /// <inheritdoc/>
    public ModClientSettings Settings { get; set; }

    public TowerManager(IBloonsApi bloonsApi, ModClientSettings settings)
    {
        BloonsApi = bloonsApi;
        Settings = settings;
    }
    
    /// <inheritdoc/>
    public double GetTotalSalary(int playerIndex, bool increaseTowerWorth = false)
    {
        var towers = BloonsApi.GetTowers(playerIndex);
        var totalSalary = 0.0;

        foreach (var tower in towers)
        {
            var salary = tower.GetSalary(Settings);
            if (increaseTowerWorth)
                tower.IncreaseWorth(salary);
            
            totalSalary += salary;
        }

        return totalSalary;
    }

    /// <inheritdoc/>
    public double SellTowers(int playerIndex, float requiredSalary)
    {
        // Get towers in ascending order.
        var towers = BloonsApi.GetTowers(playerIndex);
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