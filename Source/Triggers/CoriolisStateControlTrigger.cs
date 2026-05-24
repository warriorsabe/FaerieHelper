using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.FaerieHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FaerieHelper.Triggers;


[CustomEntity("FaerieHelper/CoriolisStateControlTrigger")]
public class CoriolisStateControlTrigger : Trigger
{
    internal int[] newStates;
    internal bool newBlacklistMode;
    internal bool resetOnExit;
    internal bool usesFlag;
    internal string activeFlag;
    
    public CoriolisStateControlTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        newStates = data.Attr("newStates").Split(',').Select(int.Parse).ToArray();
        newBlacklistMode = data.Bool("blacklistMode", false);
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

        controller.blacklistMode = newBlacklistMode;
        
        // Copied from the controller's constructor to ensure the correct helper bools are set alongside the new state list
        controller.affectNormal = false;
        controller.affectDash = false;
        foreach (int state in newStates)
        {
            if(state == 0)
                controller.affectNormal = true;
            if(state == 2)
                controller.affectDash = true;
        }
        controller.affectedStates = newStates;

        // If the trigger is non-resetting, entering a different trigger that does reset should reset to the value you had immediately before entering it rather than the original default, so said default variables are set here as well.
        if (!resetOnExit)
        {
            controller.defaultBlacklist = newBlacklistMode;
            controller.defaultStates = newStates;
            controller.defaultDash = controller.affectDash;
            controller.defaultNormal = controller.affectNormal;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        controller.blacklistMode = controller.defaultBlacklist;
        controller.affectedStates = controller.defaultStates;
        controller.affectDash = controller.defaultDash;
        controller.affectNormal = controller.defaultNormal;
    }
}