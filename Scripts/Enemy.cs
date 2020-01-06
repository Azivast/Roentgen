using Godot;
using System;

public class Enemy : KinematicBody
{
    private bool dead = false;
    
    private float movementSpeed = 10f;
    private float maxMovementSpeed = 50f;
    private float gravity = -9.82f * 0.03f;
    Vector3 velocity = Vector3.Zero;

    private Node player;
    private bool playerInFOV = false;
    private Area SeeableArea;
    private RayCast rayCast;

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
        rayCast = (RayCast)GetNode("RayCast");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // Dont process unless enemy is alive and player is present
        if (dead || playerInFOV == false || player is null) 
            return;

        // Point raycast to behind player.
        Vector3 VectorToPlayer = ((KinematicBody)player).Translation - rayCast.GlobalTransform.origin;
        VectorToPlayer *= 1.5f;
        rayCast.CastTo = VectorToPlayer;

        // Check if enemy has clear line of sight to player and if so shoot
        if (rayCast.IsColliding())
        {
            var collider = rayCast.GetCollider();

            if (collider == player) 
            {
                // Shoot
                if (firingTimer.IsStopped())
                    shoot(VectorToPlayer);
            }
        }

    }

    public override void  _PhysicsProcess(float delta)
    {
        // Apply gravity if in the air
        if (!IsOnFloor())
            velocity.y += gravity;
        else
            velocity.y = 0;

        // Dont process other physics unless enemy is alive and player is present
        if (dead || playerInFOV == false || player is null) 
        {
            // Move
            MoveAndSlide(velocity * delta);
            return;
        }
	
        // Get direction to player
        Vector3 VectorToPlayer = ((KinematicBody)player).Translation - Translation;
        Vector3 VectorToPlayerNormalized = VectorToPlayer.Normalized();
        
        // Adjust velocity to move in players direction
        velocity.x += VectorToPlayerNormalized.x * movementSpeed;
        velocity.z += VectorToPlayerNormalized.z * movementSpeed;

        // Clamp velocity
        //velocity.x = Mathf.Clamp(velocity.x, -maxMovementSpeed, maxMovementSpeed);
        //velocity.z = Mathf.Clamp(velocity.z, -maxMovementSpeed, maxMovementSpeed);

        // Move
        MoveAndSlide(velocity * delta);
    }


    private void shoot(Vector3 heading) // TODO: Use the rayCast to check if player is behind wall and if so don't fire.
    {		

        // Start timer/cooldown
        firingTimer.Start();

        // Make sure heading is normalized
        heading = heading.Normalized();

        // Spawn new bullet instance
        Node b = bullet.Instance();
        // Set current position and the direction it should travel
        ((Enemy_Bullet)b).SetPosAndHeading(GetGlobalTransform().origin, heading);
        // Add it to the bullet container
        bulletContainer.AddChild(b);
    }

    private void OnSeeableAreaEntered(Node body)
	{
		if (body == player)
        {
            playerInFOV = true;
        }
	}

    private void OnSeeableAreaExit(Node body)
    {
        if (body == player)
        {
            playerInFOV = false;
        }
    }


    public void SetPlayer(Node player)
    {
        this.player = player;
    }
}
