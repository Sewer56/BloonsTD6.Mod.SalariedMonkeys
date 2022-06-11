using BTD_Mod_Helper.Api.ModOptions;

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to Mod Helper's Setting Management.
/// </summary>
public partial class Mod
{
    // Setting Toggles (accessed via Reflection) // 
    private static readonly ModSettingDouble CostPercentPerRound = new ModSettingDouble(5.00)
    {
        displayName = "Cost Percent Per Round",
        minValue = 0.0,
        maxValue = 100.0,
        isSlider = true
    };

    private static readonly ModSettingDouble FreeplaySalaryMultiplier = new ModSettingDouble(25.00)
    {
        displayName = "Freeplay (Round 100+) Salary Percent",
        minValue = 0.0,
        maxValue = 100.0,
        isSlider = true
    };

    private static readonly ModSettingBool DisableIncome = new ModSettingBool(true)
    {
        displayName = "Disable Income",
    };

    private static readonly ModSettingBool ParagonDegreeSoldTowers = new ModSettingBool(true)
    {
        displayName = "Include Sold Towers' Pop Count in Degrees"
    };

    private static readonly ModSettingBool ParagonDegreeSoldTowersParagons = new ModSettingBool(true)
    {
        displayName = "Include Paragons in Sold Towers' Pop Count"
    };

    private static readonly ModSettingBool ShowSalaryUi = new ModSettingBool(true)
    {
        displayName = "Show Salary (Beside Cash)",
    };

    private static readonly ModSettingInt SellPenalty = new ModSettingInt((int)SellPenaltyKind.FreeBetweenRounds)
    {
        displayName = "Selling Mode (See Log)",
        isSlider = true,
        minValue = (long?) SellPenaltyKind.Always,
        maxValue = (long?) SellPenaltyKind.Free
    };

    private static readonly ModSettingInt SellPriceDisplay = new ModSettingInt((int)SellPriceDisplayMode.SalaryOnly)
    {
        displayName = "Sell Price Display Mode (See Log)",
        isSlider = true,
        minValue = (long?)SellPriceDisplayMode.TowerWorth,
        maxValue = (long?)SellPriceDisplayMode.TowerWorthAndSalary
    };

    /// <summary>
    /// Initializes all settings code on boot.
    /// </summary>
    private void Initialize_Settings()
    {
        CostPercentPerRound.OnValueChanged.Add(SetCostPerRoundFromSlider);
        DisableIncome.OnValueChanged.Add(SetDisableIncome);
        SellPenalty.OnValueChanged.Add(SetSellPenaltyType);
        ShowSalaryUi.OnValueChanged.Add(SetShowSalaryUI);
        SellPriceDisplay.OnValueChanged.Add(SetSellPriceDisplayMode);
        FreeplaySalaryMultiplier.OnValueChanged.Add(SetFreeplaySalaryMultiplier);
        ParagonDegreeSoldTowers.OnValueChanged.Add(SetParagonDegreeSoldTowers);
        ParagonDegreeSoldTowersParagons.OnValueChanged.Add(SetParagonDegreeSoldTowersParagons);

        SellPenalty.OnInitialized.Add(PrintSellPenaltyModes);
        ApplySettings();
    }

    /// <summary>
    /// Applies all mod settings on boot.
    /// </summary>
    private void ApplySettings()
    {
        SetCostPerRoundFromSlider(CostPercentPerRound);
        SetDisableIncome(DisableIncome);
        SetSellPenaltyType(SellPenalty);
        SetShowSalaryUI(ShowSalaryUi);
        SetSellPriceDisplayMode(SellPriceDisplay);
        SetFreeplaySalaryMultiplier(FreeplaySalaryMultiplier);
        SetParagonDegreeSoldTowers(ParagonDegreeSoldTowers);
        SetParagonDegreeSoldTowersParagons(ParagonDegreeSoldTowersParagons);
    }

    // Event Handlers //
    private void SetSellPenaltyType(long value) => _modSettings.SellPenalty = (SellPenaltyKind) value;
    private void SetSellPriceDisplayMode(long value) => _modSettings.SellPriceDisplayMode = (SellPriceDisplayMode)value;
    private void SetDisableIncome(bool value) => _modSettings.DisableIncome = value;
    private void SetCostPerRoundFromSlider(double d) => _modSettings.CostPercentPerRound = (float)(d / 100.0);
    private void SetFreeplaySalaryMultiplier(double d) => _modSettings.FreeplaySalaryMultiplier = (float)(d / 100.0);
    private void SetShowSalaryUI(bool value) => _modSettings.ShowSalaryInUI = value;
    private void SetParagonDegreeSoldTowers(bool value) => _modSettings.IncludeDestroyedTowersInParagonDegree = value;
    private void SetParagonDegreeSoldTowersParagons(bool value) => _modSettings.IncludeParagonsInDestroyedTowersParagonDegree = value;

    // Temporary Stuff //
    private void PrintSellPenaltyModes(SharedOption option)
    {
        Log.Always(">= About Settings Limitations =<");
        Log.NoMelon("Note: Dropdowns aren't available in Mod Helper and settings are being reworked for 3.0.\n" +
                   "It's not worth the time implementing them in current release so please bear with the sliders.\n\n" +
                   "== Sell Modes ==\n");
        
        PrintSetting(SellPenaltyKind.Always, "Selling always incurs a cost penalty.");
        PrintSetting(SellPenaltyKind.FreeBetweenRounds, "[Default] Selling is free between rounds, otherwise costs money.");
        PrintSetting(SellPenaltyKind.Free, "Selling always free.");

        Log.NoMelon("\n== Sell Price Display Modes ==");
        PrintSetting(SellPriceDisplayMode.TowerWorth, "Displays the total salary paid for the tower during the course of the game.");
        PrintSetting(SellPriceDisplayMode.SalaryOnly, "[Default] Displays the salary paid at the end of each round.");
        PrintSetting(SellPriceDisplayMode.TowerWorthAndSalary, "Displays both items above.");
    }

    private void PrintSetting<T>(T enumerable, string description) where T : Enum
    {
        Log.NoMelonNoLine(ConsoleColor.Green, $"({(int)(object)enumerable}) {enumerable}: ");
        Log.NoMelon(description);
    }
}