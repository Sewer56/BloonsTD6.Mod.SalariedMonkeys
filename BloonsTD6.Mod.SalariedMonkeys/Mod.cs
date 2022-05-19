[assembly: MelonInfo(typeof(BloonsTD6.Mod.SalariedMonkeys.TemplateMain), "Salaried Monkeys", "1.0.0", "Sewer56")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonsTD6.Mod.SalariedMonkeys;

public class TemplateMain : BloonsTD6Mod
{
    // Github API URL used to check if this mod is up to date. For example:
    public override string GithubReleaseURL => "https://api.github.com/repos/Sewer56/BloonsTD6.Mod.SalariedMonkeys/releases";

    public override void OnMainMenu()
    {
        base.OnMainMenu();
    }

    public override void OnApplicationStart()
    {
        MelonLogger.Msg("Salaried Monkeys Loaded!");
    }
}