﻿/*/*
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

using System.Reflection;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.Logging;

namespace STFixes.Detours;

public abstract class BaseHandler
{
    public string Name { get; set; }

    public abstract Enums.Detours.Mode Mode { get; }
    public abstract Models.Detour PreDetour { get; }
    public abstract Models.Detour PostDetour { get; }

    protected readonly ILogger<STFixes> _logger;
    
    public abstract void Start();
    public abstract void Stop();
    protected abstract void UnhookAllDetours();
    
    protected BaseHandler(string name, ILogger<STFixes> logger)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));;
    }
    
    private static BaseMemoryFunction GetMemoryFunction<T>() where T : BaseHandler
    {
        MethodInfo? getMemoryFunctionMethod = typeof(T).GetMethod("GetMemoryFunction");
        if(getMemoryFunctionMethod == null)
            throw new InvalidOperationException($"Class {typeof(T).Name} must define a static GetMemoryFunction method.");
        
        return getMemoryFunctionMethod.Invoke(null, null) as BaseMemoryFunction
               ?? throw new InvalidOperationException();
    }
    
    public static T Build<T>(ILogger<STFixes> logger) where T : BaseHandler
    {
        MethodInfo? buildMethod = typeof(T).GetMethod("Build", new Type[] { typeof(ILogger<STFixes>) });
        if (buildMethod == null || !buildMethod.IsStatic)
            throw new InvalidOperationException($"Class {typeof(T).Name} must define a static Build method.");
        
        return buildMethod.Invoke(null, [logger]) as T ?? throw new InvalidOperationException();
    }
}

public abstract class PreHandler : BaseHandler
{
    public override Enums.Detours.Mode Mode => Enums.Detours.Mode.Pre;
    
    public override Models.Detour PreDetour { get; }
    public override Models.Detour PostDetour 
        => throw new NotSupportedException("PostDetour is only available if Mode is set to Post or Both.");
    
    protected override void UnhookAllDetours()
    {
        if(PreDetour.IsHooked()) PreDetour.Unhook();
    }
    
    protected PreHandler(string name, Models.Detour preDetour, ILogger<STFixes> logger) : base(name, logger)
    {
        PreDetour = preDetour ?? throw new ArgumentNullException(nameof(preDetour));
    }
}

public abstract class PostHandler : BaseHandler
{
    public override Enums.Detours.Mode Mode => Enums.Detours.Mode.Post;
    
    public override Models.Detour PreDetour 
        => throw new NotSupportedException("PreDetour is only available if Mode is set to Pre or Both.");
    public override Models.Detour PostDetour { get; }
    
    protected override void UnhookAllDetours()
    {
        if(PostDetour.IsHooked()) PostDetour.Unhook();
    }
    
    protected PostHandler(string name, Models.Detour postDetour, ILogger<STFixes> logger) : base(name, logger)
    {
        PostDetour = postDetour ?? throw new ArgumentNullException(nameof(postDetour));
    }
}

public abstract class PrePostHandler : BaseHandler
{
    public override Enums.Detours.Mode Mode => Enums.Detours.Mode.Both;
    
    public override Models.Detour PreDetour { get; }
    public override Models.Detour PostDetour { get; }

    protected override void UnhookAllDetours()
    {
        if(PreDetour.IsHooked()) PreDetour.Unhook();
        if(PostDetour.IsHooked()) PostDetour.Unhook();
    }

    protected PrePostHandler(string name, Models.Detour preDetour, Models.Detour postDetour, ILogger<STFixes> logger) 
        : base(name, logger)
    {
        PreDetour = preDetour ?? throw new ArgumentNullException(nameof(preDetour));
        PostDetour = postDetour ?? throw new ArgumentNullException(nameof(postDetour));
    }
}