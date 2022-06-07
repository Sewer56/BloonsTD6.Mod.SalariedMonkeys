namespace BloonsTD6.Mod.SalariedMonkeys.Implementation;

internal class BloonsApi : IBloonsApi
{
    public static readonly BloonsApi Instance = new BloonsApi();

    public int GetPlayerIndex()
    {
        if (InGame.instance.IsCoop)
            return InGame.instance.coopGame.OwnPlayerNumber - 1;

        return 0;
    }

    public double GetCash(int playerIndex)
    {
        Log.Debug($"GETCASH: {playerIndex}");
        return InGame.instance.GetCashManager(playerIndex).cash.Value;
    }

    public void AddCash(double amount, int playerIndex = 0)
    {
        Log.Debug($"ADDCASH: {playerIndex}");
        InGame.instance.GetCashManager(playerIndex).cash.Value += amount;
        InGame.instance.bridge.OnCashChangedSim();
    }

    public List<ISalariedTower> GetTowers(int? playerIndex)
    {
        // We use this specific method because it doesn't include towers that have been destroyed.
        var towers = InGame.instance.GetAllTowerToSim();
        var list   = new List<ISalariedTower>();
        
        foreach (var tower in towers)
        {
            if (!playerIndex.HasValue || tower.tower.GetOwnerZeroBased() == playerIndex)
                list.Add(new SalariedTower(tower.tower, SalariedMonkeys.Instance.GetTowerInfo(tower.tower)));
        }
        
        return list;
    }

    public bool? ToggleSelling(bool allowSelling)
    {
        var model = InGame.instance.GetGameModel();
        if (model == null)
            return default;

        var originalSellEnabled = model.towerSellEnabled;
        model.towerSellEnabled = allowSelling;
        return originalSellEnabled;
    }

    public bool IsRoundActive() => InGame.instance.UnityToSimulation.AreRoundsActive();
    public bool IsFreeplay() => (InGame.instance.GetMap().spawner.CurrentRound + 1) > 100;
    public Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<DiscountZone>> GetDiscountInfo(Vector3 position, int path, int tier)
    {
        return InGame.instance.GetTowerManager().GetZoneDiscount(position, path, tier);
    }
}