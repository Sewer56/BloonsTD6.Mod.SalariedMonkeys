namespace BloonsTD6.Mod.SalariedMonkeys.Structures;

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