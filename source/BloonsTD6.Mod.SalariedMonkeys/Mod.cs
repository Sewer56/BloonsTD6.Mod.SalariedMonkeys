using System.Runtime.InteropServices;

[assembly: MelonInfo(typeof(BloonsTD6.Mod.SalariedMonkeys.Mod), "Salaried Monkeys", "1.0.0", "Sewer56")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to BTD6.
/// This class handles the bootstrapping part of the mod.
/// As such, it is not testable without going ingame.
/// </summary>
[ExcludeFromCodeCoverage] // game specific code
public partial class Mod : BloonsTD6Mod
{
    // Github API URL used to check if this mod is up to date. For example:
    public override string GithubReleaseURL => "https://api.github.com/repos/Sewer56/BloonsTD6.Mod.SalariedMonkeys/releases";

    // Shared parts between partial classes.
    private static IBloonsApi Api => SalariedMonkeys.Api;
    private static SalariedMonkeys SalariedMonkeys => SalariedMonkeys.Instance;
    private static ModClientSettings _modSettings = new ModClientSettings();
    private static CachedStringFormatter _cachedStringFormatter = new CachedStringFormatter();
    private static CashDisplay? _cashDisplay;
    private static int _invalidateCashDisplayTimer = 1;
    
    public override void OnTitleScreen() => Initialize_Settings();

    public override void OnMatchEnd()
    {
        // TODO: This is a hack! Find a better way to do this.
        OnMatchEnd_UI();
        OnMatchEnd_Core();
        GC.Collect();
    }

    // Executed every frame.
    public override void OnUpdate() => OnUpdate_UI();
}