using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.FaerieHelper.Entities;

[Tracked]
[CustomEntity("FaerieHelper/CoriolisController")]
public class CoriolisController : Entity
{
    internal float coriolisStrength;
    internal bool affectHorizontal;
    internal bool affectVertical;
    internal bool affectDash;
    internal bool affectGroundedDash;
    internal string activeFlag;
    internal bool usesFlag;
    
    public CoriolisController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        coriolisStrength = data.Float("strength", 5f);
        affectHorizontal = data.Bool("affectHorizontal", true);
        affectVertical = data.Bool("affectVertical", false);
        affectDash = data.Bool("affectDash", true);
        affectGroundedDash = data.Bool("affectGroundedDash", false) & affectDash;
        usesFlag = !string.IsNullOrWhiteSpace(data.Attr("coriolisFlag"));
        activeFlag = data.Attr("coriolisFlag");
    }

    public override void Update()
    {
        base.Update();
        
        if (Scene.Tracker.GetEntity<Player>() is not { } player || Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;

        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;
        
        bool playerDashing = player.StateMachine.state == 2;
        if (!(player.StateMachine.state == 0 || player.StateMachine.state == 7) && !playerDashing)
            return;
        
        if (playerDashing)
        {
            if (!controller.affectDash)
                return;
            if ((player.OnGround() || (player.SuperWallJumpAngleCheck && (player.WallJumpCheck(-1) || player.WallJumpCheck(1)))) && !controller.affectGroundedDash)
                return;
        }

        
        Vector2 playerSpeed = player.Speed;
        float coriolisForce;
        if (controller.affectVertical)
        {
            coriolisForce = playerSpeed.X * controller.coriolisStrength;
            if(coriolisForce < -900f)
            {
                coriolisForce *= Engine.DeltaTime;
                player.Speed.Y += coriolisForce;
            } else if (playerDashing && controller.affectDash) {
                if (!player.OnGround() || controller.affectGroundedDash)
                {
                    coriolisForce *= Engine.DeltaTime;
                    player.Speed.Y += coriolisForce;
                }
            }
        }

        if (controller.affectHorizontal && !(player.ClimbCheck(-1,0) && !playerDashing))
        {
            coriolisForce = playerSpeed.Y * controller.coriolisStrength;
            coriolisForce *= Engine.DeltaTime;
            player.Speed.X -= coriolisForce;
        }
    }
    
    internal static void modPlayerNormalUpdate(ILContext il) {
        
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.After, instr => instr.MatchLdcR4(900f));

        cursor.EmitLdarg0();

        cursor.EmitDelegate(changeGravity);
    }

    private static float changeGravity(float original_gravity, Player player) {

       if (player.Scene is not Level level || level.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
           return original_gravity;
       
       if (controller.usesFlag && !player.SceneAs<Level>().Session.GetFlag(controller.activeFlag))
           return original_gravity;
       
        float coriolisForce = player.Speed.X * controller.coriolisStrength;
        if (controller.affectVertical & coriolisForce > -900f)
            original_gravity += coriolisForce;
        
        return original_gravity;
    }
    
    public static void ModifiedCoroutineHook(ILContext il) 
    {
        ILCursor cursor = new ILCursor(il);
       
        if (cursor.TryGotoNext(MoveType.After,instr => instr.MatchLdfld<Player>(nameof(Player.DashDir)), instr => instr.MatchLdcR4(160f)))
        {
            cursor.Index--;
            cursor.EmitLdloc1();
            cursor.EmitDelegate(rotateVector);
        }
    }
    
    private static Vector2 rotateVector(Vector2 original_direction, Player player) {

        if (player.Scene is not Level level || level.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return original_direction;
        
        if (controller.usesFlag && !player.SceneAs<Level>().Session.GetFlag(controller.activeFlag))
            return original_direction;
        
        if(!controller.affectDash)
            return original_direction;
        
        if((player.OnGround() || (player.SuperWallJumpAngleCheck && (player.WallJumpCheck(-1) || player.WallJumpCheck(1)))) && !controller.affectGroundedDash)
            return original_direction;
        
        Logger.Log(nameof(FaerieHelperModule), "Firing dash vector rotation");
           
        float deflectionAngle = (float) Math.Atan(0.15f * controller.coriolisStrength);

       return original_direction.Rotate(deflectionAngle);
    }
    
}