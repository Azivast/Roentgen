using Godot;
using System;

public class Enemy : KinematicBody
{
    private int health = 100;
    private bool dead = false;
    private AudioStreamPlayer3D deathAudio;
    
    [Export] private float movementSpeed = 10f;
    [Export] private float maxMovementSpeed = 100f;
    private float gravity = -9.82f * 0.03f;
    Vector3 velocity = Vector3.Zero;

    private Node player;
    private bool playerInFOV = false;
    private Area SeeableArea;
    private RayCast rayCast;

    // Animation colum
    [Export] int colum;
    private Sprite3D sprite;
    private AnimationPlayer animationPlayer;


    // Shooting Variables
    private Timer firingTimer;
    private Node bulletContainer;
    private PackedScene bullet;
    private AudioStreamPlayer3D firingAudio;
    private Node muzzleFlashLight;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SeeableArea = (Area)GetNode("LineOfSight");
        firingTimer = (Timer)GetNode("FiringTimer");
        bulletContainer = GetNode("Bullet Container");
        bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullet.tscn");
        rayCast = (RayCast)GetNode("RayCast");
        sprite = (Sprite3D)GetNode("Sprite3D");
        animationPlayer = (AnimationPlayer)GetNode("Sprite3D/AnimationPlayer");
        deathAudio = (AudioStreamPlayer3D)GetNode("Death audio");
        firingAudio = (AudioStreamPlayer3D)GetNode("Firing audio");
        muzzleFlashLight = GetNode("Firing light");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // Dont process unless enemy is alive and player is present
        if (playerInFOV == false || dead || player == null) 
            return;

        // Disable muzzle flash incase it's lite
        ((OmniLight)muzzleFlashLight).Visible = false;

        // Point raycast to behind player.
        Vector3 VectorToPlayer = ((KinematicBody)player).Translation - rayCast.GlobalTransform.origin;
        // Don't do anything else if player is too far away
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
                {
                    shoot(VectorToPlayer);
                    firingAudio.Play();
                }
            }
        }

        // Animate from first row
        sprite.Frame = colum;
    }

    public override void  _PhysicsProcess(float delta)
    {
        //Apply gravity if in the air
        if (!IsOnFloor())
        {
            velocity.y += gravity;
            MoveAndSlide (velocity * delta, Vector3.Up);
        }


            
        // else
        //     velocity.y = 0;

        // Dont process other physics unless enemy is alive and player is present
        if (dead || playerInFOV == false || player == null) 
        {
            // Move
            ;
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
        MoveAndSlide(velocity * delta, Vector3.Up);
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

        // Light muzzle flash
        ((OmniLight)muzzleFlashLight).Visible = true;
    }

    private void OnSeeableAreaEntered(Node body)
	{
		if (body == player)
        {
            playerInFOV = true;
            animationPlayer.Play("walk");
        }
	}

    private void OnSeeableAreaExit(Node body)
    {
        if (body == player)
        {
            playerInFOV = false;
            animationPlayer.Seek(0, true);
            animationPlayer.Stop();

        }
    }

    public void Hit() // TODO: Implement health
    {
        health -= 33;

        if (health <= 0)
        {
            Kill();
            deathAudio.Play();
        }
        
    }
    
    public void Kill()
    {
        dead = true;
        // Disable collision
        ((CollisionShape)GetNode("CollisionShape")).Disabled = true;
        // Change to dead sprite
        sprite.Frame = 4;
    }


    public void SetPlayer(Node player)
    {
        this.player = player;
    }
}
