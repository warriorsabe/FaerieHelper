using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

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
    internal bool affectHoldables;
    internal bool affectPlayer;
    internal bool gravityLike;
    internal bool affectFeather;
    internal bool affectRedDash;
    internal bool affectDreamDash;
    internal bool affectUndefined;

    internal float defaultStrength;
    internal bool defaultHorizontal;
    internal bool defaultVertical;
    internal bool defaultDash;
    internal bool defaultGroundedDash;
    internal bool defaultHoldables;
    internal bool defaultPlayer;
    internal bool defaultGravityLike;
    internal bool defaultFeather;
    internal bool defaultRedDash;
    internal bool defaultDreamDash;
    internal bool defaultUndefined;
    internal enum AffectDashMode : byte
    {
        Legacy = 0,
        Never = 1,
        OnlyInAir = 2,
        Always = 6
    }
    internal enum DirectionMode : byte
    {
        Legacy = 0,
        Horizontal = 1,
        Vertical = 2,
        Both = 6
    }
    
    public CoriolisController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        coriolisStrength = data.Float("strength", 5f);
        affectHorizontal = data.Bool("affectHorizontal", true);
        affectVertical = data.Bool("affectVertical", true);
        affectDash = data.Bool("affectDash", true);
        affectGroundedDash = data.Bool("affectGroundedDash", false) & affectDash;
        usesFlag = !string.IsNullOrWhiteSpace(data.Attr("coriolisFlag"));
        activeFlag = data.Attr("coriolisFlag");
        affectPlayer = data.Bool("affectPlayer", true);
        affectHoldables = data.Bool("affectHoldables", false);
        gravityLike = data.Bool("gravityLike", false);
        affectFeather = data.Bool("affectFeather", false);
        affectRedDash = data.Bool("affectRedBoosters", true);
        affectDreamDash = data.Bool("affectDreamBlocks", true);
        affectUndefined = data.Bool("affectUndefined", false);
        switch (data.Enum<AffectDashMode>("affectDashMode", AffectDashMode.Legacy))
        {
            case AffectDashMode.Legacy:
                break;
            case AffectDashMode.Never:
                affectDash = false;
                affectGroundedDash = false;
                break;
            case AffectDashMode.OnlyInAir:
                affectDash = true;
                affectGroundedDash = false;
                break;
            case AffectDashMode.Always:
                affectDash = true;
                affectGroundedDash = true;
                break;
        }
        switch (data.Enum<DirectionMode>("directionMode", DirectionMode.Legacy))
        {
            case DirectionMode.Legacy:
                break;
            case DirectionMode.Horizontal:
                affectHorizontal = true;
                affectVertical = false;
                break;
            case DirectionMode.Vertical:
                affectHorizontal = false;
                affectVertical = true;
                break;
            case DirectionMode.Both:
                affectHorizontal = true;
                affectVertical = true;
                break;
        }
        defaultStrength = coriolisStrength;
        defaultHorizontal = affectHorizontal;
        defaultVertical = affectVertical;
        defaultDash = affectDash;
        defaultGroundedDash = affectGroundedDash;
        defaultHoldables = affectHoldables;
        defaultPlayer = affectPlayer;
        defaultFeather = affectFeather;
        defaultRedDash = affectRedDash;
        defaultDreamDash = affectDreamDash;
    }

    public override void Update()
    {
        base.Update();
        
        if (Scene.Tracker.GetEntity<Player>() is not { } player || Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;
        
        if (usesFlag && !SceneAs<Level>().Session.GetFlag(activeFlag))
            return;
        
        if (controller.affectHoldables && !(Scene.Tracker.GetComponents<Holdable>() is not { } holdables))
        {
            foreach (Holdable holdable in holdables)
            {
                Vector2 holdableSpeed = holdable.GetSpeed();
                float coriolisForce;
                if (controller.affectVertical)
                {
                    coriolisForce = holdableSpeed.X * controller.coriolisStrength * Engine.DeltaTime;
                    
                    if (controller.gravityLike)
                        coriolisForce *= (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f);
                    
                    holdable.SetSpeed(new Vector2(holdableSpeed.X, holdableSpeed.Y + coriolisForce));
                }
                if (controller.affectHorizontal)
                {
                    coriolisForce = holdableSpeed.Y * controller.coriolisStrength * Engine.DeltaTime;
                    
                    if (controller.gravityLike)
                        coriolisForce *= (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f);
                    
                    holdable.SetSpeed(new Vector2(holdableSpeed.X - coriolisForce, holdableSpeed.Y));
                }
            }
        }

        bool affectedByGravity = false;
        
        switch (player.StateMachine.state)
        {
            case 0:
                affectedByGravity = true;
                break;
            case 1:
                return;
            case 2:
                if(!controller.affectDash || ((player.OnGround() || (player.SuperWallJumpAngleCheck && (player.WallJumpCheck(-1) || player.WallJumpCheck(1)))) && !controller.affectGroundedDash))
                    return;
                break;
            case 5:
                if(!controller.affectRedDash || ((player.OnGround() || (player.SuperWallJumpAngleCheck && (player.WallJumpCheck(-1) || player.WallJumpCheck(1)))) && !controller.affectGroundedDash))
                    return;
                break;
            case 9:
                if(!controller.affectDreamDash)
                    return;
                break;
            case 19:
                if (!controller.affectFeather)
                    return;
                break;
            default:
                if(!controller.affectUndefined)
                    return;
                break;
        }
        
        if (controller.affectPlayer)
        {
            Vector2 playerSpeed = player.Speed;
            float coriolisForce;
            if (controller.affectVertical)
            {
                
                coriolisForce = playerSpeed.X * controller.coriolisStrength;
                
                if (controller.gravityLike)
                    coriolisForce *= (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f);
                
                if (coriolisForce < -900f * (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f))
                {
                    coriolisForce *= Engine.DeltaTime;
                    player.Speed.Y += coriolisForce;
                }
                else if (!affectedByGravity)
                {
                    coriolisForce *= Engine.DeltaTime;
                    player.Speed.Y += coriolisForce;
                }
            }

            if (controller.affectHorizontal && !(player.ClimbCheck(Math.Sign(controller.coriolisStrength), 0) && affectedByGravity))
            {
                coriolisForce = playerSpeed.Y * controller.coriolisStrength;
                
                if (controller.gravityLike)
                    coriolisForce *= (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f);
                
                coriolisForce *= Engine.DeltaTime;
                player.Speed.X -= coriolisForce;
            }
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
       
       if (!controller.affectPlayer)
           return original_gravity;
       
       if (controller.usesFlag && !player.SceneAs<Level>().Session.GetFlag(controller.activeFlag))
           return original_gravity;
       
        float coriolisForce = player.Speed.X * controller.coriolisStrength;

        if (!controller.gravityLike)
            coriolisForce /= (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f);   //Extvar will multiply these forces itself, so we undo that when we don't want it instead

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
        
        if (!controller.affectPlayer || controller.usesFlag && !player.SceneAs<Level>().Session.GetFlag(controller.activeFlag))
            return original_direction;
        
        if(!controller.affectDash)
            return original_direction;
        
        if((player.OnGround() || (player.SuperWallJumpAngleCheck && (player.WallJumpCheck(-1) || player.WallJumpCheck(1)))) && !controller.affectGroundedDash)
            return original_direction;

        float coriolisStrength = controller.coriolisStrength;
        
        if (controller.gravityLike)
            coriolisStrength *= (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f);
        
        float deflectionAngle = (float) Math.Atan(0.15f * coriolisStrength);

       return original_direction.Rotate(deflectionAngle);
    }
    
}