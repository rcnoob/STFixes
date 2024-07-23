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

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using STFixes.Fixes;
using STFixes.Fixes.Interfaces;
using Microsoft.Extensions.Logging;

namespace STFixes.Managers;

public class FixManager(PatchManager patchManager, DetourManager detourManager, EventManager eventManager,
    ILogger<STFixes> logger)
{
    private List<BaseFix> _fixes = new();

    public void OnConfigChanged(string propertyName, object? newValue)
    {
        int index = _fixes.FindIndex(fix => fix.ConfigurationProperty == propertyName);
        if (newValue == null)
        {
            StopFix(index);
            return;
        }

        if(newValue is not bool value) return;
        
        if(value) StartFix(index);
        else StopFix(index);
    }

    private void StartFix(int index)
    {
        if(index < 0 || index >= _fixes.Count) return;

        foreach(string patchName in _fixes[index].PatchNames) patchManager.PerformPatch(patchName);
        foreach(string detourHandlerName in _fixes[index].DetourHandlerNames) detourManager.StartHandler(detourHandlerName);
        foreach(var eventPair in _fixes[index].Events) eventManager.RegisterEvent(eventPair.Key, eventPair.Value);
        
        _fixes[index].Enabled = true;
    }

    private void StopFix(int index)
    {
        if(index < 0 || index >= _fixes.Count) return;
        
        foreach(string patchName in _fixes[index].PatchNames) patchManager.UndoPatch(patchName);
        foreach(string detourHandlerName in _fixes[index].DetourHandlerNames) detourManager.StopHandler(detourHandlerName);
        foreach(var eventPair in _fixes[index].Events) eventManager.UnregisterEvent(eventPair.Key, eventPair.Value);
        
        _fixes[index].Enabled = false;
    }

    public void OnTick()
    {
        List<CCSPlayerController> players = Utilities.GetPlayers();
        
        foreach (BaseFix fix in _fixes)
        {
            if(fix is ITickable tickable) tickable.OnTick(players);
        }
    }
    
    public void Start()
    {
        logger.LogInformation("[STFixes][FixManager][Start()]");
        
        _fixes.Add(new WaterFix());
        _fixes.Add(new BotNavIgnoreFix());
        _fixes.Add(new TriggerPushFix());
        _fixes.Add(new SubTickMovementFix());
    }
    
    public void Stop()
    {
        logger.LogInformation("[STFixes][FixManager][Stop()]");
        
        foreach (BaseFix fix in _fixes)
        {
            foreach(string patchName in fix.PatchNames) patchManager.UndoPatch(patchName);
            foreach(string detourHandlerName in fix.DetourHandlerNames) detourManager.StopHandler(detourHandlerName);
        }
        
        _fixes.Clear();
    }
}