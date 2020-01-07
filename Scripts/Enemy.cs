using Godot;
using System;

public class Enemy : KinematicBody
{
    private bool dead = false;
    
    private float movementSpeed = 10f;
    private float maxMovementSpeed = 100f;
    private float gravity = -9.82f * 0.03f;
    Vector3 velocity = Vector3.Zero;

    private Node player;
    private bool playerInFOV = false;
    private Area SeeableArea;
    private RayCast rayCast;

    // Shooting Variables
    private Timer firingTimer;
    private Node bulletContainer;
    private PackedScene bullet;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SeeableArea = (Area)GetNode("LineOfSight");
        firingTimer = (Timer)GetNode("FiringTimer");
        bulletContainer = GetNode("Bullet Container");
        bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullet.tscn");
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

        // Seperate horisontal velocity from vertical velocity
        Vector2 horisontalVelocity = new Vector2(velocity.x, velocity.z);

        // Clamp horisontal velocity
        horisontalVelocity = horisontalVelocity.Normalized() * Mathf.Clamp(horisontalVelocity.Length(), -maxMovementSpeed, maxMovementSpeed);

        // Merge horisontal and vertical velocity
        velocity = new Vector3(horisontalVelocity.x, velocity.y, horisontalVelocity.y);

        // Move
        MoveAndSlide(velocity * delta);
    }


    private void shoot(Vector3 heading)
    {		

        // Start timer/cooldown
        firingTimer.Start();

        // Make sure heading is normalized
        heading = heading.Normalized();

        // Spawn new bullet instance
        Node b = bullet.Instance();
        // Set speed
        ((Player_Bullet)b).Speed = 0.1f;
        // Set current position and the direction it should travel
        ((Player_Bullet)b).SetPosAndHeading(GetGlobalTransform().origin, heading);
        // Set parent
        ((Player_Bullet)b).Parent = this;
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

    public void Hit() // TODO: Implement health
    {
        Kill();
    }
    
    public void Kill()
    {
        dead = true;
    }


    public void SetPlayer(Node player)
    {
        this.player = player;
    }
}
