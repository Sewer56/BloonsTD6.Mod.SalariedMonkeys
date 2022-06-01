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

    private static readonly ModSettingBool DisableIncome = new ModSettingBool(true)
    {
        displayName = "Disable Income",
    };

    private static readonly ModSettingBool ShowSalaryUi = new ModSettingBool(true)
    {
        displayName = "Show Salary (Beside Cash)",
    };

    private static readonly ModSettingEnum<SellPenaltyKind> SellPenalty = new ModSettingEnum<SellPenaltyKind>(SellPenaltyKind.FreeBetweenRounds)
    {
        displayName = "Selling Mode",
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
        ApplySettings();
    }

    /// <summary>
    /// Applies all mod settings on boot.
    /// </summary>
    private void ApplySettings()
    {
        SetCostPerRoundFromSlider((double)CostPercentPerRound.GetValue());
        SetDisableIncome(DisableIncome);
        SetSellPenaltyType(SellPenalty);
        SetShowSalaryUI(ShowSalaryUi);
    }

    // Event Handlers //
    private void SetSellPenaltyType(SellPenaltyKind value) => _modSettings.SellPenalty = value;
    private void SetDisableIncome(bool value) => _modSettings.DisableIncome = value;
    private void SetCostPerRoundFromSlider(double d) => _modSettings.CostPercentPerRound = (float)(d / 100.0);
    private void SetShowSalaryUI(bool value) => _modSettings.ShowSalaryInUI = value;
}