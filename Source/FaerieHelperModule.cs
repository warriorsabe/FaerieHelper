using System;
using System.Reflection;
using Celeste.Mod.FaerieHelper.Entities;
using MonoMod.ModInterop;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.FaerieHelper;

public class FaerieHelperModule : EverestModule {
    public static FaerieHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(FaerieHelperModuleSettings);
    public static FaerieHelperModuleSettings Settings => (FaerieHelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(FaerieHelperModuleSession);
    public static FaerieHelperModuleSession Session => (FaerieHelperModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(FaerieHelperModuleSaveData);
    public static FaerieHelperModuleSaveData SaveData => (FaerieHelperModuleSaveData) Instance._SaveData;

    private static ILHook LoadingCoroutineHook;
    
    public FaerieHelperModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(FaerieHelperModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(FaerieHelperModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        typeof(ExtvarInterop).ModInterop();
        
        IL.Celeste.Player.NormalUpdate += CoriolisController.modPlayerNormalUpdate;
        

        MethodInfo Coroutine = typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic|BindingFlags.Instance).GetStateMachineTarget();
        
        
        LoadingCoroutineHook = new ILHook(Coroutine, CoriolisController.ModifiedCoroutineHook);
    }

    public override void Unload() {
        IL.Celeste.Player.NormalUpdate -= CoriolisController.modPlayerNormalUpdate;
        
        LoadingCoroutineHook?.Dispose(); LoadingCoroutineHook = null;
    }
}