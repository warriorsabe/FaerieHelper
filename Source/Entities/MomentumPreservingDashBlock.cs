using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.FaerieHelper.Entities;

[CustomEntity("FaerieHelper/MomentumPreservingDashBlock")]
[Tracked]
[TrackedAs(typeof(DashBlock))]
public class MomentumPreservingDashBlock : DashBlock
{
    internal float retentionTimer;
    internal float addedSpeed;
    internal bool resetCooldown;
    internal bool breakRedBubbles;
    internal bool multiplicative;
    internal string onBreakFlag;
    internal bool setsFlag;
    
    public MomentumPreservingDashBlock(EntityData data, Vector2 offset, EntityID id) : base(data, offset, id)
    {
        retentionTimer = Math.Max(data.Float("duration", 0.15f),0f);    // A negative timer shouldn't break anything, but it won't do anything different either, so might as well clamp it to zero for cleanliness
        addedSpeed = data.Float("addedSpeed", 30f);         // Default values for duration and added speed come from the corresponding values for a typical rebound
        resetCooldown = data.Bool("resetCooldown", true);
        breakRedBubbles = data.Bool("breakRedBubbles");
        multiplicative = data.Bool("multiplicative");
        onBreakFlag = data.String("onBreakFlag");
        setsFlag = !string.IsNullOrWhiteSpace(onBreakFlag);
    }
    
    internal static DashCollisionResults On_OnDashed(On.Celeste.DashBlock.orig_OnDashed orig, DashBlock self, Player player, Vector2 direction)
    {
        if (self is MomentumPreservingDashBlock block)
        {
            if (orig(self, player, direction) == DashCollisionResults.Rebound)  // Corresponds to the block being broken. Not sure why this check isn't redundant, but the blocks are indestructible without it
            {
                if(block.setsFlag)
                    block.SceneAs<Level>().Session.SetFlag(block.onBreakFlag);
                
                Vector2 playerSpeed = player.Speed; // Stored locally to avoid retrieving it multiple times
                
                if(block.multiplicative)
                    playerSpeed *= block.addedSpeed;
                else
                    playerSpeed += playerSpeed.SafeNormalize() * block.addedSpeed;  // Added speed is in the direction of travel, or opposite it if negative
                
                // While the momentum component will set the player's speed itself, doing it an additional time before it is added ensures speed can be reduced and reversed as well due to checks made by the momentum component
                player.Speed = playerSpeed;
                
                if (player.StateMachine.State != 5 || block.breakRedBubbles)
                    player.StateMachine.State = Player.StNormal;

                if (player.Get<FHDashblockMomentumComponent>() == null)     // Only add a new component if there isn't one, to avoid unnecessarily stacking several copies of the component
                    player.Add(new FHDashblockMomentumComponent(block.retentionTimer, playerSpeed, block.onBreakFlag));
                else
                    player.Get<FHDashblockMomentumComponent>().setValues(block.retentionTimer, playerSpeed, block.onBreakFlag);

                if(block.resetCooldown)
                    player.dashCooldownTimer = 0f;
            
                if (player.StateMachine.State != 5 || block.breakRedBubbles)
                    return DashCollisionResults.NormalOverride;    
            }
        }
        return orig(self, player, direction);
    }
}