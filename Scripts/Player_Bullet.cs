using Godot;
using System;

public class Player_Bullet : Area
{
    [Export] private int speed = 10;
	private Vector3 velocity = new Vector3();
	// Direction of travel
	private Vector3 heading;
	public override void _Process(float delta)
  	{
		// Move bullet
		SetTranslation(GetTranslation() + heading);
  	}

	public void SetPosAndHeading(Vector3 position, Vector3 heading)
	{
		SetTranslation(position);
		heading.Normalized();
		this.heading = heading;
	}

	private void OnLifeTimeTimeout()
	{
		RemoveBullet();
	}

	private void OnBodyEntered(Node body)
	{
		if (body.Name == "Player") {}

		else if (body.HasMethod("Kill"))
		{
			//((Enemy)body).Kill();
			RemoveBullet();
		}

		else
			RemoveBullet();
	}

	private void RemoveBullet()
	{
		// Safely remove node
		QueueFree();
	}
}
