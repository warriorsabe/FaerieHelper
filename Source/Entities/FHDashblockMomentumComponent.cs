using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
namespace Celeste.Mod.FaerieHelper.Entities;

internal class FHDashblockMomentumComponent : Component
{
    private float momentumTimer;
    private Vector2 retainedMomentum;
    private List<string> storedBlockFlags;
    
    public FHDashblockMomentumComponent(float duration, Vector2 speed, string flag) : base(active: true, visible: false)
    {
        momentumTimer = duration;
        retainedMomentum = speed;
        
        // Momentum dash block-set flags are meant to be temporary, so the momentum component is co-opted to keep track of the flags set so they can be unset later even without the dash block still present
        storedBlockFlags = new List<string>();
        if (!string.IsNullOrWhiteSpace(flag))
            storedBlockFlags.Add(flag);
    }

    public void setValues(float duration, Vector2 speed, string flag)   // Does everything the constructor does (sans list initialization) so that modifying an existing component is the same as adding a new one
    {
        momentumTimer = duration;
        retainedMomentum = speed;
        if (!string.IsNullOrWhiteSpace(flag))
            storedBlockFlags.Add(flag);
    }

    public override void Update()
    {
        base.Update();

        if (Scene.Tracker.GetEntity<Player>() is not { } player)
            return;
        
        // Checks to run if timer is active instead of removing self when timer expires in order to allow the component to act as sufficiently persistent storage for the list of flags
        if (momentumTimer > 0 && retainedMomentum != Vector2.Zero)
        {
            momentumTimer -= Engine.DeltaTime;
            
            if (player.StateMachine.state != 2) // Dashing shouldn't remove the retained speed entirely, but we don't want it interfering with the dash either
            {
                // Don't prevent jumping off walls or floors with retained momentum, but also keep whichever component doesn't interfere with that
                if (Math.Sign(retainedMomentum.X) != Math.Sign(player.Speed.X) && player.Speed.X != 0)
                    retainedMomentum.X *= 0;
                if (Math.Sign(retainedMomentum.Y) != Math.Sign(player.Speed.Y) && player.Speed.Y != 0)
                    retainedMomentum.Y *= 0;
                // Don't prevent the player from gaining extra speed
                player.Speed.X = (float) Math.MaxMagnitude(retainedMomentum.X, player.Speed.X);
                player.Speed.Y = (float) Math.MaxMagnitude(retainedMomentum.Y, player.Speed.Y);
            }
        }
        else if(momentumTimer <= 0)
            retainedMomentum = Vector2.Zero;
    }

    public static void removeMomentumDashBlockFlags(Player player)  // Called on death
    {
        if(player.Get<FHDashblockMomentumComponent>() is FHDashblockMomentumComponent component)
            foreach (string flag in component.storedBlockFlags)
                player.SceneAs<Level>().Session.SetFlag(flag, false);
    }
    public static void removeMomentumDashBlockFlags(Level level, LevelData levelData, Vector2 position) // Different header to be called on screen transition
    {
        if(level.Tracker.GetEntity<Player>().Get<FHDashblockMomentumComponent>() is FHDashblockMomentumComponent component)
            foreach (string flag in component.storedBlockFlags)
                level.Session.SetFlag(flag, false);
    }
}