using System;
using System.Reflection;
using Celeste.Mod.FaerieHelper.Entities;
using Celeste.Mod.FaerieHelper.ModInterop;
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
        Logger.SetLogLevel(nameof(FaerieHelperModule), LogLevel.Verbose);
#else
        Logger.SetLogLevel(nameof(FaerieHelperModule), LogLevel.Info);
#endif
    }

    public override void Load()
    {
        typeof(ExtvarInterop).ModInterop();
        typeof(GravityHelperInterop).ModInterop();
        typeof(Exports).ModInterop();

        Everest.Events.Player.OnDie += FHDashblockMomentumComponent.removeMomentumDashBlockFlags;
        Everest.Events.Level.OnTransitionTo += FHDashblockMomentumComponent.removeMomentumDashBlockFlags;
        
        IL.Celeste.Player.NormalUpdate += CoriolisController.modPlayerNormalUpdate;

        On.Celeste.DashBlock.OnDashed += MomentumPreservingDashBlock.On_OnDashed;
        

        MethodInfo Coroutine = typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic|BindingFlags.Instance).GetStateMachineTarget();
        
        
        LoadingCoroutineHook = new ILHook(Coroutine, CoriolisController.ModifiedCoroutineHook);
    }

    public override void Unload()
    {
        Everest.Events.Player.OnDie -= FHDashblockMomentumComponent.removeMomentumDashBlockFlags;
        Everest.Events.Level.OnTransitionTo -= FHDashblockMomentumComponent.removeMomentumDashBlockFlags;
        
        IL.Celeste.Player.NormalUpdate -= CoriolisController.modPlayerNormalUpdate;

        On.Celeste.DashBlock.OnDashed -= MomentumPreservingDashBlock.On_OnDashed;
        
        LoadingCoroutineHook?.Dispose(); LoadingCoroutineHook = null;
    }
}