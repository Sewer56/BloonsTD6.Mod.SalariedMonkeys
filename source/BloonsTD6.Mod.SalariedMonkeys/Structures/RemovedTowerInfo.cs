namespace BloonsTD6.Mod.SalariedMonkeys.Structures;

public class RemovedTowerInfo
{
    /// <summary>
    /// ID of the base tower.
    /// e.g. Ninja for 0-2-0 Ninja.
    /// </summary>
    public string BaseTowerId { get; set; } = "";

    /// <summary>
    /// Pops for this tower.
    /// </summary>
    public long Pops { get; set; }

    // For serializers
    public RemovedTowerInfo() { }

    public RemovedTowerInfo(Tower tower)
    {
        Pops = tower.damageDealt;
        BaseTowerId = tower.towerModel.baseId;
    }
}