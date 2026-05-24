using System;
using Celeste.Mod.Entities;
using Celeste.Mod.FaerieHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FaerieHelper.Triggers;

[CustomEntity("FaerieHelper/CoriolisBooleanTrigger")]
public class CoriolisBooleanTrigger : Trigger
{
    internal bool newValue;
    internal bool resetOnExit;
    internal bool usesFlag;
    internal string activeFlag;

    internal enum TargetType
    {
        UsesFlag, AffectHoldables, AffectPlayer, GravityLike, DashTechProtection
    }

    internal TargetType target;
    
    public CoriolisBooleanTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        newValue = data.Bool("newValue", true);
        resetOnExit = data.Bool("resetOnExit", true);
        target = data.Enum<TargetType>("targetValue", TargetType.AffectPlayer);
        
        activeFlag = data.Attr("flag");
        usesFlag = !string.IsNullOrWhiteSpace(activeFlag);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;

        // If the trigger is non-resetting, entering a different trigger that does reset should reset to the value you had immediately before entering it rather than the original default, so said default variables are set here as well.
        // This logic is included within each of the relevant switch cases.
        switch (target)
        {
            case TargetType.UsesFlag:
                controller.usesFlag = newValue;
                if(!resetOnExit)
                    controller.defaultFlag = newValue;
                break;
            case TargetType.AffectHoldables:
                controller.affectHoldables = newValue;
                if(!resetOnExit)
                    controller.defaultHoldables = newValue;
                break;
            case TargetType.AffectPlayer:
                controller.affectPlayer = newValue;
                if(!resetOnExit)
                    controller.defaultPlayer = newValue;
                break;
            case TargetType.GravityLike:
                controller.gravityLike = newValue;
                if(!resetOnExit)
                    controller.defaultGravityLike = newValue;
                break;
            case TargetType.DashTechProtection:
                controller.dashTechProtection = newValue;
                if(!resetOnExit)
                    controller.defaultDashTechProtection = newValue;
                break;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        // In case multiple resetting boolean triggers are overlapped, it should only reset the value modified by this trigger
        switch (target)
        {
            case TargetType.UsesFlag:
                controller.usesFlag = controller.defaultFlag;
                break;
            case TargetType.AffectHoldables:
                controller.affectHoldables = controller.defaultHoldables;
                break;
            case TargetType.AffectPlayer:
                controller.affectPlayer = controller.defaultPlayer;
                break;
            case TargetType.GravityLike:
                controller.gravityLike = controller.defaultGravityLike;
                break;
            case TargetType.DashTechProtection:
                controller.dashTechProtection = controller.defaultDashTechProtection;
                break;
        }
    }
}