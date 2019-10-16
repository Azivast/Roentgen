using Godot;
using System;

public class Player_Bullet : Area
{
    [Export] private int speed = 10;
	private Vector3 velocity = new Vector3();
	[Export] public Vector3 Heading = new Vector3();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
	
	public override void _Process(float delta)
  	{
		//AngularVelocity = Heading * speed * delta;
  	}

    public void SetHeading(Vector3 heading)
  	{
		Heading = heading;
  	}
}
