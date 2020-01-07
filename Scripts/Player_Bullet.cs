using Godot;
using System;

public class Player_Bullet : RigidBody
{
    public float Speed = 1;
	private Vector3 velocity = new Vector3();
	// Direction of travel
	private Vector3 heading;

	private PackedScene hitParticle;
	private PackedScene enemyHitParticle;

	public Node Parent;

	public override void _Ready()
	{
		hitParticle = ResourceLoader.Load<PackedScene>("res://Scenes/Hit Particle.tscn");
		enemyHitParticle = ResourceLoader.Load<PackedScene>("res://Scenes/Enemy Hit Particle.tscn");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Move bullet
		SetTranslation(GetTranslation() + heading);
	}

	public void SetPosAndHeading(Vector3 position, Vector3 heading)
	{
		SetTranslation(position);
		heading = heading.Normalized();
		this.heading = heading*Speed;
	}

	private void OnLifeTimeTimeout()
	{
		RemoveBullet();
	}

	private void OnBodyEntered(Node body)
	{
		Node b = hitParticle.Instance();
		heading = Vector3.Zero;

		if (body == Parent) {}

		else if (body.HasMethod("Hit"))
		{
			((Enemy)body).Hit();
			heading = Vector3.Zero;
			Node p = hitParticle.Instance();
			AddChild(p);
		}

		else
		{
			heading = Vector3.Zero;
			Node p = hitParticle.Instance();
			AddChild(p);
			RemoveBullet();
		}
	}

	private void RemoveBullet()
	{
		// Safely remove node
		QueueFree();
	}
}
