﻿using BloonsTD6.Mod.SalariedMonkeys.Interfaces;

namespace BloonsTD6.Mod.SalariedMonkeys.Tests.Mocks;

internal class FakeBloonsApi : IBloonsApi
{
    public double Cash;
    public List<ISalariedTower> Towers;
    public bool AllowSelling;
    public int TowersSold;
    public bool IsRoundActiveValue;
    public float DifficultyCostMultiplier = 1.00f;

    public FakeBloonsApi(double cash, List<ISalariedTower> towers)
    {
        Cash = cash;
        Towers = towers;
    }

    public double GetCash() => Cash;

    public void AddCash(double amount) => Cash += amount;

    public List<ISalariedTower> GetTowers() => Towers;

    public bool? ToggleSelling(bool allowSelling)
    {
        var original = AllowSelling;
        AllowSelling = allowSelling;
        return original;
    }

    public bool IsRoundActive() => IsRoundActiveValue;
    public float GetDifficultyCostMultiplier() => DifficultyCostMultiplier;
}