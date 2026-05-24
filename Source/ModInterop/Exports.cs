using Celeste.Mod.FaerieHelper.Entities;
using Monocle;
using MonoMod.ModInterop;

namespace Celeste.Mod.FaerieHelper.ModInterop;

public static class Exports
{
    [ModExportName("FaerieHelper")]
    public static class FaerieHelperExports
    {
        public static string GetCoriolisFlag(Scene scene)
        {
            if (scene.Tracker.GetEntity<CoriolisController>() is CoriolisController controller)
                return controller.activeFlag;
            
            return null;
        }
        public static int[] GetCoriolisAffectedStates(Scene scene)
        {
            if (scene.Tracker.GetEntity<CoriolisController>() is CoriolisController controller)
                return controller.affectedStates;
            
            return null;
        }
        public static float GetCoriolisStrength(Scene scene)
        {
            if (scene.Tracker.GetEntity<CoriolisController>() is CoriolisController controller)
                return controller.coriolisStrength;     // Will return strength as used internally, in units of radians per second clockwise, not the units used by the lönn plugin
            
            return 0.0f;
        }
        public static bool GetCoriolisBoolean(Scene scene, string valueName)
        {
            if (scene.Tracker.GetEntity<CoriolisController>() is CoriolisController controller)
            {
                switch (valueName)
                {
                    case "usesFlag":
                        return controller.usesFlag;
                    case "gravityLike":
                        return controller.gravityLike;
                    case "dashTechProtection":
                        return controller.dashTechProtection;
                    case "affectHoldables":
                        return controller.affectHoldables;
                    case "affectPlayer":
                        return controller.affectPlayer;
                }
            }
            return false;
        }
    }
}