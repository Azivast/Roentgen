using Godot;
using System;

public class Player_Bullet : KinematicBody
{
    public float Speed = 1f;
	private Vector3 velocity = new Vector3();
	// Direction of travel
	private Vector3 heading;

	private PackedScene hitParticle;
	private PackedScene enemyHitParticle;

	public Node Parent;
	private CollisionShape parentCollisionShape;

	public override void _Ready()
	{
		hitParticle = ResourceLoader.Load<PackedScene>("res://Scenes/Hit Particle.tscn");
		enemyHitParticle = ResourceLoader.Load<PackedScene>("res://Scenes/Enemy Hit Particle.tscn");
		parentCollisionShape = (CollisionShape)Parent.GetNode("CollisionShape");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Move bullet and get collider
		var collision = MoveAndCollide(heading);

		// Handle collision if bullet is colliding
		try 
		{
			if (collision.Collider != null)
				HandleCollision(collision);
		}
		catch
		{
			return;
		}
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

	private void HandleCollision(KinematicCollision collision)
	{
		if (collision.Collider == Parent)
			return;

		
		heading = Vector3.Zero;
		Node b = hitParticle.Instance();
		((CPUParticles)b.GetNode("CPUParticles")).Emitting = true;

		if (collision.Collider.HasMethod("Hit"))
		{
			try 
			{
				((Enemy)collision.Collider).Hit();
				((Enemy)collision.Collider).AddChild(b);
				RemoveBullet();
			}
			catch
			{
				((Player)collision.Collider).Hit();
				((Player)collision.Collider).AddChild(b);
				RemoveBullet();
			}
		}

		else
		{
			AddChild(b);
			RemoveBullet();
		}
	}

	private void RemoveBullet()
	{
		// Safely remove node
		QueueFree();
	}
}
