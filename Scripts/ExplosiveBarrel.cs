using Godot;
using System;

public class ExplosiveBarrel : RigidBody
{
    private bool hasExploded = false;
    private AnimationPlayer animation;
    private AudioStreamPlayer3D audio;
    private Sprite3D sprite;
    private Timer timer;
    private Area area;
    private CollisionShape collisionShape;
    private MeshInstance explosionMesh;

    public override void _Ready()
	{
		animation = ((AnimationPlayer)GetNode("AnimationPlayer"));
        audio = ((AudioStreamPlayer3D)GetNode("AudioStreamPlayer3D"));
        sprite = (Sprite3D)GetNode("Barrel");
        timer = (Timer)GetNode("Timer");
        area = (Area)GetNode("Explosion Area");
        collisionShape = (CollisionShape)GetNode("CollisionShape");
        explosionMesh = (MeshInstance)GetNode("Explosion mesh");
	}


    public void Hit()
    {
        if (hasExploded) return;
        hasExploded = true;
        animation.Play("Explode");
        audio.Play();
        sprite.QueueFree();
        timer.Start();
    }

    public void OnExplosionAreaBodyEntered(Node body)
    {
        if (body.HasMethod("Hit"))
            body.Call("Hit");
        if (body.HasMethod("Kill"))
            body.Call("Kill");   
    }

    private void RemoveExploded()
    {
        area.QueueFree();
        collisionShape.QueueFree();
        explosionMesh.QueueFree();
    }
    private void RemoveAll()
    {
        QueueFree();
    }
}
