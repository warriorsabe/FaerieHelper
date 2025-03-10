using Celeste.Mod.Entities;
using Celeste.Mod.FaerieHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FaerieHelper.Triggers;

[CustomEntity("FaerieHelper/CoriolisDirectionModeTrigger")]
public class CoriolisDirectionModeTrigger : Trigger
{
    internal bool newAffectHorizontal;
    internal bool newAffectVertical;
    internal bool resetOnExit;
    internal bool usesFlag;
    internal string activeFlag;
    
    internal enum AffectDirectionMode : byte
    {
        Horizontal = 1,
        Vertical = 2,
        Both = 6
    }
    
    public CoriolisDirectionModeTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        resetOnExit = data.Bool("resetOnExit", true);
        usesFlag = !string.IsNullOrWhiteSpace(data.Attr("flag"));
        activeFlag = data.Attr("flag");
        switch (data.Enum<AffectDirectionMode>("newDirectionMode", AffectDirectionMode.Both))
        {
            case AffectDirectionMode.Horizontal:
                newAffectHorizontal = true;
                newAffectVertical = false;
                break;
            case AffectDirectionMode.Vertical:
                newAffectHorizontal = false;
                newAffectVertical = true;
                break;
            case AffectDirectionMode.Both:
                newAffectHorizontal = true;
                newAffectVertical = true;
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

        controller.affectVertical = newAffectVertical;
        controller.affectHorizontal = newAffectHorizontal;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (!resetOnExit || Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;
        
        controller.affectVertical = controller.defaultVertical;
        controller.affectHorizontal = controller.defaultHorizontal;
    }
}