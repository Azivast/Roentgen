using Godot;
using System;

public class Enemy : KinematicBody
{
    private Node Player;

    private bool seesPlayer = false;

    private Area SeeableArea;

    // Shooting Variables
    private Timer firingTimer;
    private Node bulletContainer;
    [Export] private PackedScene bullet;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SeeableArea = (Area)GetNode("LineOfSight");
        firingTimer = (Timer)GetNode("FiringTimer");
        bulletContainer = GetNode("Bullet Container");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (seesPlayer)
            if (firingTimer.IsStopped())
                shoot();
    }

    private void shoot()
    {		
        // Start timer/cooldown
        firingTimer.Start();

        // Calculate direction in which to fire (towards player)
        // No need to normalize as the bullet class does that anyhow
        Vector3 heading = ((KinematicBody)Player).GetGlobalTransform().origin - GetGlobalTransform().origin;

        // Spawn new bullet instance
        Node b = bullet.Instance();
        // Set current position and the direction it should travel
        ((Enemy_Bullet)b).SetPosAndHeading(GetGlobalTransform().origin, heading);
        // Add it to the bullet container
        bulletContainer.AddChild(b);
    }

    private void OnSeeableAreaEntered(Node body)
	{
		if (body.Name == "Player")
        {
            seesPlayer = true;

            // Link body to local player variable
            if (Player == null)
                Player = body;
        }
	}

    private void OnSeeableAreaExit(Node body)
    {
        if (body.Name == "Player")
        {
            seesPlayer = false;
        }
    }
}
