using Newtonsoft.Json;

namespace BloonsTD6.Mod.SalariedMonkeys;

public class RemovedTowerTracker
{
    /// <summary>
    /// Entry name in the map save metadata.
    /// </summary>
    public const string MetadataEntryName = "SalariedMonkeysRemovedTowers";

    [JsonProperty("Towers")]
    public List<RemovedTowerInfo> Towers { get; private set; } = new List<RemovedTowerInfo>();

    private Dictionary<string, List<RemovedTowerInfo>> _baseIdToTower = new();

    /// <summary>
    /// Adds a tower for tracking.
    /// </summary>
    /// <param name="tower">The tower to add.</param>
    public void AddTower(Tower tower)
    {
        var towerInfo = new RemovedTowerInfo(tower);
        Towers.Add(towerInfo);
        AddToLookupSets(towerInfo);
    }

    /// <summary>
    /// Returns a list of towers with a given base id.
    /// </summary>
    public List<RemovedTowerInfo> GetFromBaseId(string baseId) => _baseIdToTower.GetOrCreateItem(baseId);

    /// <summary>
    /// Returns a mapping of based ID to tower.
    /// </summary>
    public Dictionary<string, List<RemovedTowerInfo>> GetIdToTowerMap() => _baseIdToTower;

    // Save/Load Logic
    public string Serialize() => JsonConvert.SerializeObject(this);

    public static RemovedTowerTracker Deserialize(string text)
    {
        var item = JsonConvert.DeserializeObject<RemovedTowerTracker>(text) ?? new RemovedTowerTracker();

        foreach (var tower in item.Towers)
            item.AddToLookupSets(tower);

        return item;
    }

    private void AddToLookupSets(RemovedTowerInfo towerInfo)
    {
        var towersById = _baseIdToTower.GetOrCreateItem(towerInfo.BaseTowerId);
        towersById.Add(towerInfo);
    }
}