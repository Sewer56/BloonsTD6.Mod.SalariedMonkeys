# Monkeys for Hire in Bloons TD 6

## Introduction

Usually in a tower defense game, you buy a tower and you upgrade a tower. You pay once, and you get to keep your prized posessions forever. However, in Bloons; most of the towers are monkeys; living, breathing creatures; so someone's gotta pay them in the monkey economy, right? 

Introducing Monkeys for Hire, a mod that changes Bloons such that the monkeys are paid contractors as opposed to fixed cost towers.

## About the Mod

Monkeys for Hire is a custom game mode; which applies over all existing game modes.  

### Core Rules
- At the end of each round, monkeys are paid a salary based on their total cost (+ upgrades).  
- You can see this salary counter on the top of the screen (beside cash display).  
- Selling towers is free in between rounds, however incurs the penalty of 1 round's worth of salary if performed mid-round.  
- Once the player is in debt, towers will be forcefully sold until the player can once again afford them.  
- Passing Round 100 enters freeplay 'deflation' mode, where tower costs are altered such that you're slowly expected to burn your remaining cash.  
- In order to win, player must complete final round with a positive amount of cash.  
    - In co-op, the sum of all players' cash must be positive.  

- Income is disabled.  

Some core rules are configurable, please see `Mod Settings`.

### Notable Behaviours
- When hitting debt, towers are sold in increasing order of cost (from the cheapest), until salary decreases by the same amount of cash as you are in debt.
    - In co-op, players have their own separate salaries and only have their own towers sold, as such it's possible to strategically sell by transferring money.  

- Adora's XP Bonus (& Paragon degree bonus) are based on the total amount of salary paid to the tower.  
    - This persists between saved games (if played with mod).  
    - Sacrificing long term, mid game carrying towers that fall off late game can yield large XP bonus.  

- Blood Sacrifice (Adora) uses the same rules as selling.  
    - Cash penalty for mid-round sacrifice.  
    - The cash penalty still counts towards XP given.  
    - Free between rounds.  

- In ***Boss Events***, some extra rules apply.  
    - Rounds 40, 60, 80, 100 and 120 are free, you don't pay salaries for them.  

- Paragons have been slightly altered:  
    - Paragons are discountable [to balance boss events].  
    - Pops from sold (and previously sacrificed) non-paragon towers are included when calculating paragon degree.  

- Tier 4s are discountable!  

### Trivia

- This is (probably) the first custom game mode with co-op support.  
    - This mod is responsible for fixing and expanding [Mod Helper's networking capabilities](https://github.com/gurrenm3/BTD-Mod-Helper/pull/42).  
    - I made the [Multi-User](https://github.com/Sewer56/BloonsTD6.Mod.MultiUser) to test this mod, allowing people to co-op with themselves on one computer.  
  
- It's my first mod for a Unity based game (and Bloons).  

### Strategy Hints (Spoiler!)
<details>
    
- By default, the cost of a tower is paid over the course of 20 rounds.  
    - Anything bought more than 20 rounds before last round is more expensive.  
    - Anything bought in the last 20 rounds can be considerably cheaper.  

- In ***CHIMPS***, save up for Freeplay (Round > 100)!  
    - It is recommended to have around $30,000 spare cash.  

</details>

## Features List

✅ indicates a feature is implemented.  
⚠️ indicates a feature is not yet complete.  
❌ indicates a feature is not yet implemented.  

- ✅ Mod Settings.  
- ✅ Custom Freeplay 'Deflation' Mode.  
- ✅ Extended CHIMPS Mode.  
- ✅ Cost Scaling per Difficulty.  
- ✅ Accurate costs/salary in Menus.  
- ✅ Combines with Other Mods.  
- ✅ Disable Income.  
- ✅ Sacrifices (Adora & Paragons).  
- ⚠️ Co-Op (Implemented but not extensively tested).  
- ❌ Setting presets (Needs mod helper update!).  
- ❌ Geraldo (Thinking about best course of action for it).  

## Tested GameModes
- CHIMPS (140 Rounds)  
- Impoppable (140 Rounds)  
- Boss Event 'Bloonarius' Elite  
    - Very difficult. Good luck.  

## Download

You can find the download for the mod in the [Releases Section on GitHub](https://github.com/Sewer56/BloonsTD6.Mod.SalariedMonkeys/releases/latest).  
Simply extract the DLL to your `Mods` directory.  

## Programmers: Building

All source code is contained inside the `source` folder.  

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
