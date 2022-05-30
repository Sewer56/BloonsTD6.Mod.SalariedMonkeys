using BloonsTD6.Mod.SalariedMonkeys.Utilities;
using Il2CppNewtonsoft.Json;
using NinjaKiwi.Players;
using NinjaKiwi.Players.Files;
using UnhollowerBaseLib;

namespace BloonsTD6.Mod.SalariedMonkeys;

internal static class ProfileSwitcher
{
    /// <summary>
    /// Name of the profile to use for this application.
    /// </summary>
    public static string ProfileName { get; private set; } = "";

    public const string IdentityFileName = "identity";
    public const string SaveFileName = "Profile.Save";

    private static NativeHook<Hooks.FileStorage_LoadFn> _filestorageLoadHook = Hooks.LoadFromFileStorage.Hook(LoadFromFileStorageImpl).Activate();

    /// <summary>
    /// Initializes this class instance.
    /// </summary>
    public static void Initialize()
    {
        var commandline = Environment.GetCommandLineArgs();
        for (int x = 0; x < commandline.Length; x++)
        {
            if (commandline[x] == "--profile")
                ProfileName = SanitizeFileName(commandline[x + 1]);
        }
    }

    public static IntPtr LoadFromFileStorageImpl(Il2CppNativeHookVariable<string> path,
        Il2CppNativeHookVariable<PasswordGenerator> passwordGenerator,
        Il2CppNativeHookVariable<JsonSerializerSettings> jsonSettings,
        SaveStrategy saveStrategy,
        bool ignoreIfNotReadable,
        IntPtr method)
    {
        var filePath = path.GetObject();

        if (filePath != null && filePath.EndsWith(IdentityFileName))
        {
            var newFileName = Path.Combine(Path.GetDirectoryName(filePath), $"{IdentityFileName}-{ProfileName}");
            path.RawValue   = IL2CPP.ManagedStringToIl2Cpp(newFileName);
            MelonLogger.Msg($"Redirecting Identity file: {newFileName}");
        }

        return _filestorageLoadHook.OriginalFunction(path, passwordGenerator, jsonSettings, saveStrategy, ignoreIfNotReadable, method);
    }

    /// <summary>
    /// Sanitizes a file name such that it can be written to a file.
    /// </summary>
    private static string SanitizeFileName(this string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(fileName.Where(x => !invalidChars.Contains(x)).ToArray());
    }
}