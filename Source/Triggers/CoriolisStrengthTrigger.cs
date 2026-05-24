using System;
using Celeste.Mod.Entities;
using Celeste.Mod.FaerieHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FaerieHelper.Triggers;


[CustomEntity("FaerieHelper/CoriolisStrengthTrigger")]
public class CoriolisStrengthTrigger : Trigger
{
    internal float newStrength;
    internal bool resetOnExit;
    internal bool usesFlag;
    internal string activeFlag;
    
    public CoriolisStrengthTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        newStrength = data.Float("newStrength", -2.5f);
        newStrength *= (float) Math.PI / -180f;  // Change units from degrees per second counterclockwise to radians per second clockwise
        resetOnExit = data.Bool("resetOnExit", true);
        
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
        
        controller.coriolisStrength = newStrength;
        // If the trigger is non-resetting, entering a different trigger that does reset should reset to the value you had immediately before entering it rather than the original default, so said default variable is set here as well.
        if(!resetOnExit)
            controller.defaultStrength = newStrength;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        controller.coriolisStrength = controller.defaultStrength;
    }
}