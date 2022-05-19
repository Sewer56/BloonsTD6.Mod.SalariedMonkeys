# Salaried Monkeys for Bloons TD 6

## Building

### First Time Setup

- Install [.NET 6.0 SDK or newer](https://dotnet.microsoft.com/en-us/download).  

Compiled game mod should appear in the `Mods` directory in your game folder.

### Building from Command Line
- Open a terminal (cmd/powershell/bash) and navigate to the project folder.
- Run `dotnet build -c Release`.

### Building from Visual Studio
- Install Visual Studio 2022 (or Newer).
- Open `BloonsTD6.Mod.SalariedMonkeys.sln`.

### Copy on Build

In order to make it so your mod gets copied to the `Mods` folder on build, you need to set the `BLOONSTD6_PATH` Environment Variable.

- For most users simply double clicking `SetEnvironmentVariable.ps1` should be sufficient.
- Alternatively, run the script manually from PowerShell `.\SetEnvironmentVariable.ps1 -Path "D:\Games\BloonsTD6"`.  


## Updating Game & Library Versions 

Rather than hardcoding game paths, this project builds using `Reference Assemblies`.  
Reference Assemblies are DLLs which contain API implementations only; i.e. the bare minimum to build the mod.  

This makes it so the mod can be compiled without having a local copy of Bloons installed; such as in a `GitHub Actions` environment.

- Install `MelonLoader` and `BTD6 Mod Helper`, run Bloons TD6 at least once.
- Execute the `MakeReferenceAssemblies.ps1` script in PowerShell.
    - If needed, you can specify a custom game path as a parameter, e.g. `.\MakeReferenceAssemblies.ps1 -Path D:\Games\Steam\steamapps\common\BloonsTD6`
