using System;
using MonoMod.ModInterop;

namespace Celeste.Mod.FaerieHelper;

[ModImportName("ExtendedVariantMode")]
public static class ExtvarInterop
{
    public static Func<string, object> GetCurrentVariantValue;
}