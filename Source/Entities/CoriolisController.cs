using System;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.FaerieHelper.Entities;

[Tracked]
[CustomEntity("FaerieHelper/CoriolisController")]
public class CoriolisController : Entity
{
    // Variables set by options in the entity's lonn plugin
    
    internal float coriolisStrength;    // Units of radians per second
    internal string activeFlag;
    internal bool usesFlag;
    internal bool gravityLike;
    internal bool dashTechProtection;
    internal bool affectHoldables;
    internal bool affectPlayer;
    internal bool blacklistMode;
    internal int[] affectedStates;
    //  Helper bools for state-specific logic later
    internal bool affectDash;  
    internal bool affectNormal;

    // Copies of the above variables initialized to the same value, stored so triggers that reset on exit can properly reset the controller
    
    internal float defaultStrength;
    internal bool defaultFlag;
    internal bool defaultDashTechProtection;
    internal bool defaultHoldables;
    internal bool defaultPlayer;
    internal bool defaultGravityLike;
    internal bool defaultBlacklist;
    internal int[] defaultStates;
    internal bool defaultDash;
    internal bool defaultNormal;

    // Just so Space Station Detour doesn't get broken by the improved physics
    internal bool legacyBehavior;
    
    public CoriolisController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        // Legacy values, not editable on newly placed entities
        
        legacyBehavior = true;
        legacyBehavior = data.Bool("legacyBehavior", true);
        
        dashTechProtection = !data.Bool("affectGroundedDash", false);
        affectDash = data.Bool("affectDash", false);
        
        // Customizable values
        
        coriolisStrength = data.Float("strength", 5f);
        activeFlag = data.Attr("coriolisFlag");
        usesFlag = !string.IsNullOrWhiteSpace(activeFlag);   // Not directly customizable, but auto-detects whether a flag is set
        if(!usesFlag)
            activeFlag = "";    // Standardize base case for consistent exports
        affectPlayer = data.Bool("affectPlayer", true);
        affectHoldables = data.Bool("affectHoldables", false);
        gravityLike = data.Bool("gravityLike", false);
        if (!legacyBehavior)
            affectedStates = data.Attr("affectedStates").Split(',').Select(int.Parse).ToArray();
        dashTechProtection = data.Bool("dashTechProtection", true);
        blacklistMode = data.Bool("blacklistMode", false);

        if (legacyBehavior)
        {
            if (affectDash)
                affectedStates = [0, 2];
            else
                affectedStates = [0];
        }


        // Set helper bools based on list of affected states
        affectNormal = false;
        affectDash = false;
        foreach (int state in affectedStates)
        {
            if (state == 0)
                affectNormal = true;
            if(state == 2)
                affectDash = true;
        }
        
        // Change units from degrees per second counterclockwise to radians per second clockwise

        if (!legacyBehavior)
            coriolisStrength *= (float) Math.PI / -180f;
        
        // Copy values to defaults for use by triggers
        
        defaultStrength = coriolisStrength;
        defaultFlag = usesFlag;
        defaultDashTechProtection = dashTechProtection;
        defaultHoldables = affectHoldables;
        defaultPlayer = affectPlayer;
        defaultStates = affectedStates;
        defaultBlacklist = blacklistMode;
        defaultNormal = affectNormal;
        defaultDash = affectDash;
    }

    // Main Coriolis logic
    
    public override void Update()
    {
        base.Update();
        
        if (Scene.Tracker.GetEntity<Player>() is not { } player || Scene.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return;    // No coriolis forces if no coriolis controller
        
        if (controller.usesFlag && !SceneAs<Level>().Session.GetFlag(controller.activeFlag))
            return;     // No coriolis forces if the controller is set to use a flag which is disabled


        // Coriolis forces attempt to rotate the current velocity vector at a specific angular velocity
        float rotationAngle = controller.coriolisStrength * Engine.DeltaTime;
        
        // Coriolis effect on dropped holdables
        
        if (controller.affectHoldables && Scene.Tracker.GetComponents<Holdable>() is { } holdables)
        {
            foreach (Holdable holdable in holdables)
            {
                Vector2 holdableSpeed = holdable.GetSpeed();
                
                // Inverts rotation direction when gravityhelper inverts gravity, if gravitylike is true
                if (controller.gravityLike && (GravityHelperInterop.IsActorInverted?.Invoke(holdable.EntityAs<Actor>()) ?? false))
                    rotationAngle *= -1f;
                
                Vector2 rotatedHoldableSpeed = holdableSpeed.Rotate(rotationAngle);
                
                holdable.SetSpeed(rotatedHoldableSpeed);
                
                if (controller.gravityLike && (GravityHelperInterop.IsActorInverted?.Invoke(holdable.EntityAs<Actor>()) ?? false))
                    rotationAngle *= -1f;   // Put rotation angle back to normal so inversions don't persist to subsequent uses of the variable
            }
        }
        
        // Coriolis forces on player
        
        if (controller.affectPlayer)
        { 
            Vector2 playerSpeed = player.Speed; // Stored locally in order to safely perform temporary modifications and avoid needing to retrieve it several times

            if (playerSpeed == Vector2.Zero)
                return;     // No coriolis forces happen at zero speed, so skip unnecessary logic and prevent possible issues from trying to operate on zero vectors
            
            // Player state control logic
        
            bool affectedByGravity = false;
            
            if (controller.legacyBehavior)
            {
                switch (player.StateMachine.state)
                {
                    case 0:
                        affectedByGravity = true;
                        break;
                    case 1:
                        return;
                    case 2:
                        if ((player.OnGround() || (player.SuperWallJumpAngleCheck && (player.WallJumpCheck(-1) || player.WallJumpCheck(1)))) && controller.dashTechProtection)
                            return; // Don't pull the player away from surfaces they could super, hyper, or wallbounce off of unless affect dash mode is "Always"
                        break;
                    default:
                        return;
                }
            }
            else
            {
                bool active = controller.blacklistMode;
                foreach (int state in controller.affectedStates)
                {
                    if (state == player.StateMachine.state)
                    {
                        active = !active;
                        break;
                    }
                }
                if (!active)
                    return;
            }

            bool inverted = GravityHelperInterop.IsPlayerInverted?.Invoke() ?? false;
            
            // Same tech protection check as above, but also checks for red bubble state in addition to dash state. Ignores disabled tech protection if it would rotate a grounded dash into the ground to avoid stacking an ultraboost every frame
            bool willCollideIfDash = player.OnGround() && (!inverted && Math.Sign(player.Speed.X) == Math.Sign(controller.coriolisStrength) || (inverted && Math.Sign(player.Speed.X) == -1 * Math.Sign(controller.coriolisStrength)));
            if (player.StateMachine.state is 2 or 5 && (player.OnGround() || (player.SuperWallJumpAngleCheck && (player.WallJumpCheck(-1) || player.WallJumpCheck(1)))) && (controller.dashTechProtection || willCollideIfDash))
                return;
            
            if(player.StateMachine.state == 0)
                affectedByGravity = true;

            
            // Determined that the player is in a state the coriolis controller is set to affect
            // Actual physics logic follows
            
            if(inverted)
                playerSpeed.Y *= -1f;
            
            if (controller.gravityLike && inverted)
                rotationAngle *= -1f;    // Similar to inversion of holdable rotation
            
            Vector2 rotatedSpeed = playerSpeed.Rotate(rotationAngle);
            
            if (!controller.legacyBehavior && rotatedSpeed.Y > player.maxFall)
                player.maxFall = rotatedSpeed.Y;
            
            float coriolisForce;     // Stored as a velocity offset rather than directly applying rotated velocity so that forces can be compared to gravity, and to more easily support legacy behavior
            
            // Vertical Speed Adjustment
            
            if (controller.legacyBehavior)
                coriolisForce = playerSpeed.X * controller.coriolisStrength * Engine.DeltaTime;
            else
                coriolisForce = rotatedSpeed.Y - playerSpeed.Y;
            
            if (!affectedByGravity || coriolisForce < -900f * Engine.DeltaTime * (float)(ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f))
            {
                if(inverted)
                    coriolisForce *= -1f;
                
                player.Speed.Y += coriolisForce;    // Only directly apply coriolis forces vertically if they would overcome gravity, otherwise allow gravity adjustment to take over role
            }

            // Horizontal Speed Adjustment

            if (controller.legacyBehavior)
            {
                if (!(player.ClimbCheck(Math.Sign(controller.coriolisStrength), 0) && affectedByGravity)) // Prevents you from getting pulled away from walls you try to climb
                {
                    coriolisForce = playerSpeed.Y * controller.coriolisStrength * Engine.DeltaTime;

                    player.Speed.X -= coriolisForce;
                }
            }
            else
            {
                if (player.wallSlideDir == 0) // Smarter version of legacy check
                {
                    coriolisForce = rotatedSpeed.X - playerSpeed.X;

                    if (player.wallSpeedRetentionTimer == 0 || controller.legacyBehavior)
                        player.Speed.X += coriolisForce;
                }

                // Normalize dash direction, ensuring dream block entry and performed techs match the direction madeline is moving
                
                if (controller.affectDash && player.StateMachine.state is 2 or 5 or 9)
                {
                    Vector2 newDashDir = player.Speed.SafeNormalize();
                    
                    if(controller.dashTechProtection && player.DashDir.Y > 0 && newDashDir.Y <= 0)
                        newDashDir.Y = player.DashDir.Y;    // Stops downright dashes which get bent upwards from killing speed like an upwards dash if dash tech protection is enabled
                        
                    if(player.wallSpeedRetentionTimer == 0) // Ensures that dashing straight into a wall doesn't allow you to wallbounce
                        player.DashDir = newDashDir.SafeNormalize();
                }
            }
        }
    }
    
    // ILHook for Gravity-based vertical speed adjustment (applies when coriolis force acts downwards or is weaker than gravity, and the player is under the influence of gravity)
    internal static void modPlayerNormalUpdate(ILContext il) {
        
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.After, instr => instr.MatchLdcR4(900f));

        cursor.EmitLdarg0();

        cursor.EmitDelegate(changeGravity);
    }

    // Gravity-based vertical speed adjustment logic
    private static float changeGravity(float originalGravity, Player player) {

       if (player.Scene is not Level level || level.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
           return originalGravity;
       
       if (!controller.affectPlayer || !controller.affectNormal || player.Speed == Vector2.Zero)
           return originalGravity;
       
       if (controller.usesFlag && !player.SceneAs<Level>().Session.GetFlag(controller.activeFlag))
           return originalGravity;
       
       // Recalculate vertical component of coriolis force
       
        float coriolisForce; 
        
        if(controller.legacyBehavior)
            coriolisForce = player.Speed.X * controller.coriolisStrength;
        else
        {
            float rotationAngle = controller.coriolisStrength * Engine.DeltaTime;
            if (GravityHelperInterop.IsPlayerInverted?.Invoke() ?? false)
                coriolisForce = -1f * ((-1f * player.Speed).Rotate(-1f * rotationAngle).Y + player.Speed.Y);    // Gravity helper summons negations georg
            else
                coriolisForce = player.Speed.Rotate(rotationAngle).Y - player.Speed.Y;
            
            coriolisForce /= Engine.DeltaTime;
            // Multiplication by deltatime will happen after this modified gravity value is returned to the stack, but deltatime cannot be removed from the calculation here,
            // so it is divided out after the coriolis offset is calculated to undo the extraneous later multiplication
        }

        coriolisForce /= (float) (ExtvarInterop.GetCurrentVariantValue?.Invoke("Gravity") ?? 1f);   //Extvar will multiply these forces itself, so we undo that too
        
        if (coriolisForce > -900f)  // While it looks different due to the modifications to the coriolisForce value, this is the exact opposite of check on the normal vertical coriolis logic, ensuring this happens when that doesn't 
            originalGravity += coriolisForce;
        
        return originalGravity;
    }
    
    // ILHook for dash exit direction fix
    public static void ModifiedCoroutineHook(ILContext il) 
    {
        ILCursor cursor = new ILCursor(il);
       
        if (cursor.TryGotoNext(MoveType.After,instr => instr.MatchLdfld<Player>(nameof(Player.DashDir)), instr => instr.MatchLdcR4(160f)))
        {
            cursor.Index--;
            cursor.EmitLdloc1();
            cursor.EmitDelegate(CorrectedDashExit);
        }
    }
    
    // Logic for dash exit direction fix
    // Forces dash exit momentum to be in the same direction as the actual movement when a coriolis controller is modifying dashes,
    // thus avoiding the unnatural abrupt changes in direction that would otherwise happen if the dash gets strongly curved by coriolis
    private static Vector2 CorrectedDashExit(Vector2 originalDirection, Player player) {

        if (player.Scene is not Level level || level.Tracker.GetEntity<CoriolisController>() is not CoriolisController controller)
            return originalDirection;
        
        if (!controller.affectPlayer || !controller.affectDash || (controller.usesFlag && !player.SceneAs<Level>().Session.GetFlag(controller.activeFlag)))
            return originalDirection;

        return player.Speed.SafeNormalize();
    }
}