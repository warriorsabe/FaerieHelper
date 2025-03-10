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
        AffectHorizontal, AffectVertical, AffectHoldables, AffectPlayer, AffectRedDash, AffectDreamDash, AffectFeather, AffectUndefined, GravityLike
    }

    internal TargetType target;
    
    public CoriolisBooleanTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        newValue = data.Bool("newValue", true);
        resetOnExit = data.Bool("resetOnExit", true);
        target = data.Enum<TargetType>("targetValue", TargetType.AffectPlayer);
        usesFlag = !string.IsNullOrWhiteSpace(data.Attr("flag"));
        activeFlag = data.Attr("flag");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;

        switch (target)
        {
            case TargetType.AffectHorizontal:
                controller.affectHorizontal = newValue;
                break;
            case TargetType.AffectVertical:
                controller.affectVertical = newValue;
                break;
            case TargetType.AffectHoldables:
                controller.affectHoldables = newValue;
                break;
            case TargetType.AffectPlayer:
                controller.affectPlayer = newValue;
                break;
            case TargetType.AffectRedDash:
                controller.affectRedDash = newValue;
                break;
            case TargetType.AffectDreamDash:
                controller.affectDreamDash = newValue;
                break;
            case TargetType.AffectFeather:
                controller.affectFeather = newValue;
                break;
            case TargetType.AffectUndefined:
                controller.affectUndefined = newValue;
                break;
            case TargetType.GravityLike:
                controller.gravityLike = newValue;
                break;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (!resetOnExit || Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;
        
        switch (target)
        {
            case TargetType.AffectHorizontal:
                controller.affectHorizontal = controller.defaultHorizontal;
                break;
            case TargetType.AffectVertical:
                controller.affectVertical = controller.defaultVertical;
                break;
            case TargetType.AffectHoldables:
                controller.affectHoldables = controller.defaultHoldables;
                break;
            case TargetType.AffectPlayer:
                controller.affectPlayer = controller.defaultPlayer;
                break;
            case TargetType.AffectRedDash:
                controller.affectRedDash = controller.defaultRedDash;
                break;
            case TargetType.AffectDreamDash:
                controller.affectDreamDash = controller.defaultDreamDash;
                break;
            case TargetType.AffectFeather:
                controller.affectFeather = controller.defaultFeather;
                break;
            case TargetType.AffectUndefined:
                controller.affectUndefined = controller.defaultUndefined;
                break;
            case TargetType.GravityLike:
                controller.gravityLike = controller.defaultGravityLike;
                break;
        }
    }
}