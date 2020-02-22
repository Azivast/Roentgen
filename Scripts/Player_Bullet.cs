using Godot;
using System;

public class Player_Bullet : KinematicBody
{
    public float Speed = 1f;
	private Vector3 velocity = new Vector3();
	// Direction of travel
	private Vector3 heading;

	private PackedScene hitParticle;
	private PackedScene humanHitParticle;

	public Node Parent;
	private CollisionShape parentCollisionShape;

	public override void _Ready()
	{
		hitParticle = ResourceLoader.Load<PackedScene>("res://Scenes/Hit Particle.tscn");
		humanHitParticle = ResourceLoader.Load<PackedScene>("res://Scenes/Human Hit Particle.tscn");
		parentCollisionShape = (CollisionShape)Parent.GetNode("CollisionShape");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Move bullet and get collider
		var collision = MoveAndCollide(heading, false);

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

		if (collision.Collider.HasMethod("Hit"))
		{
			Node b = new Node();

			GD.Print("A");
			// Set hit particle based on living or inanimate object
			if (collision.Collider is Player || collision.Collider is Enemy)
				b = humanHitParticle.Instance();
			else
				b = hitParticle.Instance();
			GD.Print("b");
			// Set particle variables
			((CPUParticles)b.GetNode("CPUParticles")).Emitting = true;

			Vector3 offset = Vector3.Zero;
			if (collision.Collider.Get("Translation") == null) {}
			else offset = (Vector3)collision.Collider.Get("Translation");
			((CPUParticles)b.GetNode("CPUParticles")).SetTranslation(collision.Position - offset);
			
			if (collision.Collider is RigidBody)
			{
				// Apply impulse to rigidbodies
				((RigidBody)collision.Collider).ApplyCentralImpulse(-collision.Normal * 5f);
			}


			collision.Collider.Call("Hit");
			// Add particle
			((Node)collision.Collider).AddChild(b);
			RemoveBullet();
		}

		else
		{
			Node b = hitParticle.Instance();
			((CPUParticles)b.GetNode("CPUParticles")).Emitting = true;

			Vector3 offset = Vector3.Zero;
			if (collision.Collider.Get("Translation") == null) {}
			else offset = (Vector3)collision.Collider.Get("Translation");
			((CPUParticles)b.GetNode("CPUParticles")).SetTranslation(collision.Position - offset);
			((Node)collision.Collider).AddChild(b);
			RemoveBullet();
		}
	}

	private void RemoveBullet()
	{
		// Safely remove node
		QueueFree();
	}
}
