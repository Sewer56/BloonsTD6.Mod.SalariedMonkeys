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

    /// <summary>
    /// True if the tower is a paragon, else false.
    /// </summary>
    public bool IsParagon { get; set; }

    // For serializers
    public RemovedTowerInfo() { }

    public RemovedTowerInfo(Tower tower)
    {
        Pops = tower.damageDealt;
        var model = tower.towerModel;
        BaseTowerId = model.baseId;
        IsParagon = model.isParagon;
    }
}