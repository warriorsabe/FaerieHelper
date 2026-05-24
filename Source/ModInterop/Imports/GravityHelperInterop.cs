using System;
using MonoMod.ModInterop;

namespace Celeste.Mod.FaerieHelper;

[ModImportName("GravityHelper")]
public static class GravityHelperInterop
{
    public static Func<bool> IsPlayerInverted;
    
    public static Func<Actor, bool> IsActorInverted;
}