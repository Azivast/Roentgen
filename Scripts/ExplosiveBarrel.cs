using Godot;
using System;

public class ExplosiveBarrel : RigidBody
{
    private bool hasExploded = false;
    private AnimationPlayer animation;
    private AudioStreamPlayer3D audio;
    private Sprite3D sprite;

    public override void _Ready()
	{
		animation = ((AnimationPlayer)GetNode("AnimationPlayer"));
        audio = ((AudioStreamPlayer3D)GetNode("AudioStreamPlayer3D"));
        sprite = (Sprite3D)GetNode("Barrel");
	}


    public void Hit()
    {
        if (hasExploded) return;
        hasExploded = true;
        animation.Play("Explode");
        audio.Play();
        sprite.QueueFree();
    }

    public void OnExplosionAreaBodyEntered(Node body)
    {
        if (body.HasMethod("Hit"))
            body.Call("Hit");
        if (body.HasMethod("Kill"))
            body.Call("Kill");   
    }
    // Triggered when timer finishes
    private void Remove()
    {
        QueueFree();
    }
}
