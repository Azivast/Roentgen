using Godot;
using System;

public class Enemy : KinematicBody
{
    private Node Player;

    private bool playerInFOV = false;
    private bool player = false;
    private Area SeeableArea;
    private RayCast rayCast;

    // The pathfollow parent node that the enemy follows
    private PathFollow path;
    [Export] private float movementSpeed = 0.1f;

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
        rayCast = (RayCast)GetNode("LineOfSight/RayCast");

        // Get parent node and save it in path variable if it is a path
        Node parentNode = GetParent();
        if (parentNode is PathFollow)
            path = (PathFollow)parentNode;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (playerInFOV)
        {
            // Point raycast thowards player
            rayCast.SetCastTo(((KinematicBody)Player).GetGlobalTransform().origin);
            // Check if enemy has clear line of sight to player
            var collider = rayCast.GetCollider();
            if (collider == Player || collider == null) 
            {
                // Shoot
                if (firingTimer.IsStopped())
                    shoot();
            }
        }


        // Move the enemy along the path if there is one
        try {
        path.Offset += movementSpeed;
        }
        // Nothing needs to happen if it failes
        catch {}
    }

    private void shoot() // TODO: Use the rayCast to check if player is behind wall and if so don't fire.
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
            // Link body to local player variable
            if (Player == null)
                Player = body;

            playerInFOV = true;
        }
	}

    private void OnSeeableAreaExit(Node body)
    {
        if (body.Name == "Player")
        {
            playerInFOV = false;
        }
    }
}
