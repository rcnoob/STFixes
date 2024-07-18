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

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using Microsoft.Extensions.Logging;

namespace STFixes.Managers;

public class EventManager
{
    private readonly ILogger<STFixes> _logger;
    
    private Dictionary<string, List<STFixes.GameEventHandler>> _events = new();
    
    public EventManager(ILogger<STFixes> logger)
    {
        _logger = logger;
    }
    
    public void Start()
    {
        _events.Add("OnRoundStart", new List<STFixes.GameEventHandler>());
        _events.Add("OnPlayerSpawn", new List<STFixes.GameEventHandler>());
        _events.Add("OnPlayerTeam", new List<STFixes.GameEventHandler>());
        _events.Add("OnRoundStartPre", new List<STFixes.GameEventHandler>());
    }
    
    public void Stop()
    {
        foreach (var eventPair in _events)
        {
            eventPair.Value.Clear();
        }
        _events.Clear();
    }
    
    public void RegisterEvent(string name, STFixes.GameEventHandler gameEventHandler)
    {
        if(_events.TryGetValue(name, out List<STFixes.GameEventHandler>? handlers))
        {
            handlers.Add(gameEventHandler);
        }
    }
    
    public void UnregisterEvent(string name, STFixes.GameEventHandler gameEventHandler)
    {
        if(_events.TryGetValue(name, out List<STFixes.GameEventHandler>? handlers))
        {
            handlers.Remove(gameEventHandler);
        }
    }

    public HookResult ProcessEvent(GameEvent @event, GameEventInfo info, string eventName)
    {
        HookResult returnValue = HookResult.Continue;

        foreach(STFixes.GameEventHandler handler in _events[eventName])
        {
            switch(handler(@event, info, _logger))
            {
                case HookResult.Continue:
                    continue;
                case HookResult.Changed:
                    returnValue = HookResult.Changed;
                    break;
                case HookResult.Handled:
                    returnValue = HookResult.Handled;
                    break;
                case HookResult.Stop:
                    return HookResult.Stop;
            }
        }

        return returnValue;
    }
    
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info) =>
        ProcessEvent(@event, info, "OnRoundStart");
    
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info) =>
        ProcessEvent(@event, info, "OnPlayerSpawn");
    
    public HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info) =>
        ProcessEvent(@event, info, "OnPlayerTeam");
    
    public HookResult OnRoundStartPre(EventRoundPrestart @event, GameEventInfo info) =>
        ProcessEvent(@event, info, "OnRoundStartPre");
}