using Celeste.Mod.Entities;
using Celeste.Mod.FaerieHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FaerieHelper.Triggers;

[CustomEntity("FaerieHelper/CoriolisDashModeTrigger")]
public class CoriolisDashModeTrigger : Trigger
{
    internal bool newDash;
    internal bool newGroundedDash;
    internal bool resetOnExit;
    internal bool usesFlag;
    internal string activeFlag;
    
    internal enum AffectDashMode : byte
    {
        Never = 1,
        OnlyInAir = 2,
        Always = 6
    }
    
    public CoriolisDashModeTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        resetOnExit = data.Bool("resetOnExit", true);
        usesFlag = !string.IsNullOrWhiteSpace(data.Attr("flag"));
        activeFlag = data.Attr("flag");
        switch (data.Enum<AffectDashMode>("newDashMode", AffectDashMode.Never))
        {
            case AffectDashMode.Never:
                newDash = false;
                newGroundedDash = false;
                break;
            case AffectDashMode.OnlyInAir:
                newDash = true;
                newGroundedDash = false;
                break;
            case AffectDashMode.Always:
                newDash = true;
                newGroundedDash = true;
                break;
        }
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;

        controller.affectDash = newDash;
        controller.affectGroundedDash = newGroundedDash;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (!resetOnExit || Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;
        
        controller.affectDash = controller.defaultDash;
        controller.affectGroundedDash = controller.defaultGroundedDash;
    }
}