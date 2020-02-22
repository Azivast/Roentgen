using Godot;
using System;

public class Box : RigidBody
{
    // Number of times box has been hit.
    private int hitCount = 0;
    // Number of times box can be hit before it breaks;
    [Export] private int maxHitCount = 3;
    
    public void Hit()
    {
        hitCount++;

        if (hitCount >= maxHitCount)
        {
            ((AudioStreamPlayer)GetNode("Break")).Play();
            GetNode("CollisionShape").QueueFree();
            ((Timer)GetNode("Removal timer")).Start();
        }
        else
            ((AudioStreamPlayer)GetNode("Hit")).Play();
            
    }

    // Triggered when removal timer finishes
    private void Remove()
    {
        QueueFree();
    }
}
