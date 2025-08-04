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

using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using Microsoft.Extensions.Logging;

namespace STFixes;

public partial class STFixes
{
    public FakeConVar<bool> EnableWaterFix = new("css_fixes_water_fix", "Fixes being stuck to the floor underwater, allowing players to swim up.", true);
    public FakeConVar<bool> EnableBotNavIgnoreFix = new("css_bot_ignore_nav_fix", "Fixes bots not spawning without navmesh", true);
    public FakeConVar<bool> EnableTriggerPushFix = new("css_fixes_trigger_push_fix", "Reverts trigger_push behaviour to that seen in CS:GO.", true);
    public FakeConVar<bool> DisableSubTickMovement = new("css_fixes_disable_sub_tick_movement", "Disables sub-tick movement.", false);
    
    private void RegisterConVars()
    {
        EnableWaterFix.ValueChanged += (sender, value) => { _configuration.EnableWaterFix = value; };
        EnableBotNavIgnoreFix.ValueChanged += (sender, value) => { _configuration.EnableBotNavIgnoreFix = value; };
        EnableTriggerPushFix.ValueChanged += (sender, value) => { _configuration.EnableTriggerPushFix = value; };
        DisableSubTickMovement.ValueChanged += (sender, value) => { _configuration.DisableSubTickMovement = value; };
        
        RegisterFakeConVars(typeof(ConVar));
    }
}