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
        resetOnExit = data.Bool("resetOnExit", true);
        
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
        
        controller.coriolisStrength = newStrength;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (!resetOnExit || Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        controller.coriolisStrength = controller.defaultStrength;
    }
}