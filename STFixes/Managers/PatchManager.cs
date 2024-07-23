/*
    =============================================================================
    CS#Fixes
    Copyright (C) 2023-2024 Charles Barone <CharlesBarone> / hypnos <hyps.dev>
    =============================================================================

    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU General Public License, version 3.0, as published by the
    Free Software Foundation.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
    details.

    You should have received a copy of the GNU General Public License along with
    this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory;
using STFixes.Models;
using Microsoft.Extensions.Logging;

namespace STFixes.Managers;

public class PatchManager
{
    private List<Patch> _patches = new();
    
    private readonly GameDataManager _gameDataManager;
    private readonly ILogger<STFixes> _logger;
    
    public PatchManager(GameDataManager gameDataManager, ILogger<STFixes> logger)
    {
        _gameDataManager = gameDataManager;
        _logger = logger;
    }

    public void Start()
    {
        LoadCommonPatches();
    }
    
    public void Stop()
    {
        foreach (Patch patch in _patches)
        {
            patch.UndoPatch();
        }
        
        _patches.Clear();
    }

    // https://github.com/Source2ZE/CS2Fixes/blob/main/gamedata/cs2fixes.games.txt
    private void LoadCommonPatches()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Water Fix
            AddServerPatch("FixWaterFloorJump", "CheckJumpButtonWater", "11 43");
            AddServerPatch("WaterLevelGravity", "WaterLevelGravity", "3C 02");
            AddServerPatch("CategorizeUnderwater", "CategorizeUnderwater", "0F 42");
            AddServerPatch("BotNavIgnore", "BotNavIgnore", "\\xE9\\x15\\x00\\x00\\x00");
        }
        else
        {
            // Water Fix
            AddServerPatch("FixWaterFloorJump", "CheckJumpButtonWater", "11 43");
            AddServerPatch("WaterLevelGravity", "WaterLevelGravity", "3C 02");
            AddServerPatch("CategorizeUnderwater", "CategorizeUnderwater", "73");
            AddServerPatch("BotNavIgnore", "BotNavIgnore", "\\xE9\\x2C\\x00\\x00\\x00\\x90");
        }
    }
    
    private void AddServerPatch(string name, string signature, string bytesHex)
    {
        _patches.Add(new Patch(name, Addresses.ServerPath, signature, bytesHex, _gameDataManager, _logger));
    }
    

    public void PerformPatch(string name)
    {
        // Find Patch by name
        int patch = _patches.FindIndex(patch => patch.GetPatchName() == name);
        if (patch == -1)
        {
            _logger.LogError(
                "[STFixes][PatchManager][PerformPatch()][Patch={patchName}] Error: Patch not found.",
                patch);
            return;
        }
        
        _patches[patch].PerformPatch();
        
        // Commented out since it seems to cause crashes every time I test it...
        // According to src code of CS2Fixes, we should run BotNavIgnore patch 3 times on Linux???
        /* if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && name == "BotNavIgnore")
        {
            int patch2 = _patches.FindIndex(patch2 => patch2.GetPatchName() == "BotNavIgnore2");
            _patches[patch2].PerformPatch();
            int patch3 = _patches.FindIndex(patch3 => patch3.GetPatchName() == "BotNavIgnore3");
            _patches[patch3].PerformPatch();
        } */
    }
    
    public void UndoPatch(string name)
    {
        // Find Patch by name
        int patch = _patches.FindIndex(patch => patch.GetPatchName() == name);
        if (patch == -1)
        {
            _logger.LogError(
                "[STFixes][PatchManager][UndoPatch()][Patch={patchName}] Error: Patch not found.",
                patch);
            return;
        }
        
        // Commented out since it seems to cause crashes every time I test it...
        // According to src code of CS2Fixes, we should run BotNavIgnore patch 3 times on Linux???
        /* if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && name == "BotNavIgnore")
        {
            int patch3 = _patches.FindIndex(patch3 => patch3.GetPatchName() == "BotNavIgnore3");
            _patches[patch3].UndoPatch();
            int patch2 = _patches.FindIndex(patch2 => patch2.GetPatchName() == "BotNavIgnore2");
            _patches[patch2].UndoPatch();
        } */
        
        _patches[patch].UndoPatch();
    }
}