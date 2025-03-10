using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using CoroutineHolder = On.Monocle.CoroutineHolder;

namespace Celeste.Mod.FaerieHelper.Entities;

[CustomEntity("FaerieHelper/MomentumPreservingDashBlock")]
[Tracked(false)]
[TrackedAs(typeof(DashBlock))]
public class MomentumPreservingDashBlock : DashBlock
{
    
    public MomentumPreservingDashBlock(EntityData data, Vector2 offset, EntityID id) : base(data, offset, id)
    {
    }
    
    internal static DashCollisionResults On_OnDashed(On.Celeste.DashBlock.orig_OnDashed orig, DashBlock self, Player player, Vector2 direction)
    {
        if (self is MomentumPreservingDashBlock block)
        {
            if (orig(self, player, direction) == DashCollisionResults.Rebound)
            {
                ModifiedOnDash((MomentumPreservingDashBlock) self, player, direction);
                return DashCollisionResults.NormalOverride;
            }
        }
        return orig(self, player, direction);
    }

    private static void ModifiedOnDash(MomentumPreservingDashBlock self, Player player, Vector2 direction)
    {
        player.StateMachine.State = Player.StNormal;
    }
    
}