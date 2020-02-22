using Godot;
using System;

public class Player : KinematicBody
{
	// Movement variables
	Vector3 direction;
	Vector3 velocity;

	public Vector3 Velocity
	{ 
		get
		{
			return velocity;
		}
	}
	public Vector3 Direction
	{ 
		get
		{
			return direction;
		}
	}

	// Life variables
	[Export] public int MaxHealth = 100;
	public int Health;
	private AudioStreamPlayer addHealthSound;

	// HUD variables
	private Label HealthLabel;
	private Label AmmoLabel;

	// Mouse variables
	[Export] public static float mouseSensitivity = 0.003f;
	private Vector2 mouseMovement;

	// Raycast for interacting with objects
	private RayCast raycast;

	// Fly variables
	private float flySpeed = 40f;
	private float flyAccel = 1f;
	private bool flying = false;

	// Walk variables
	private float gravity = -9.82f / 60;
	public float maxWalkSpeed = 1.5f;
	// TODO: This variable doesn't work for some damn reason
	// Searh for "maxSprintSpeed" further down and set it manually instead
	private float maxSprintSpeed = 3f;
	private float accel = 2f;
	private float deAccel = 10f;

	// Jumping variables
	private float jumpHeight = 5;
	private bool onGround = false;
	private RayCast onGroundRaycast;

	// Slope variables
	private int maxSlopAngle = 25;

  // Shooting variables
	[Export] private int ammo = 0;
	[Export] private Node gunParticles;
	[Export] private Position3D firingPosition;
	private PackedScene bullet;
	[Export] private Timer firingTimer;
	private Node bulletContainer;
	private AudioStreamPlayer firingSound;
	private AudioStreamPlayer hitSound;
	private AudioStreamPlayer addAmmoSound;

	private Spatial head;
	// Where the player is looking
	private Basis aim;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
		{
			raycast = (RayCast)GetNode("Head/Camera/RayCast");
			onGroundRaycast = (RayCast)GetNode("Head/GroundRayCast");
			head =  (Spatial)GetNode("Head");
			bulletContainer = GetNode("Bullet Container");
			bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullet.tscn");
			firingPosition = (Position3D)GetNode("Head/Gun/Firing Position");
			firingTimer = (Timer)GetNode("Head/Gun/FiringTimer");
			HealthLabel = (Label)GetNode("HUD/Health/Text");
			AmmoLabel = (Label)GetNode("HUD/Ammo/Text");
			firingSound = (AudioStreamPlayer)GetNode("Firing audio");
			hitSound = (AudioStreamPlayer)GetNode("Hit audio");
			addAmmoSound = (AudioStreamPlayer)GetNode("Ammo audio");
			addHealthSound = (AudioStreamPlayer)GetNode("Health audio");
			
			// Set life to max life
			Health = MaxHealth;

			// Set Player variable for all enemies
			GetTree().CallGroup("Enemies", "SetPlayer", this);
		}
	
	public override void _Input(InputEvent @event)
	{
			// Rotate
			if (@event is InputEventMouseMotion)
			{
				mouseMovement = ((InputEventMouseMotion)@event).Relative;
			}
	}	

	private void _Interact()
	{
		var collider = raycast.GetCollider();
	
		try 
		{
			// if (collider.HasMethod("Interact")) 
			// {
			// 	Console.WriteLine("Door opened.");
			// 	((WinButton)collider).Interact();
			// 	((Door)collider).Interact();
				
			// }
			if (collider is Door)
			{
				Console.WriteLine("Door opened.");
				((Door)collider).Interact();	
			}
			else if (collider is WinButton)
			{
				((WinButton)collider).Interact();
			}
		}

		catch
		{
			return;
		}
	}	

	private void look()
	{
		if (mouseMovement.Length() != 0)
		{
			head.Rotate(Vector3.Up, -mouseMovement.x * mouseSensitivity);
			head.RotateObjectLocal(Vector3.Left, mouseMovement.y * Player.mouseSensitivity);

			// Clamp rotation so that player can't turn their head upside down
			// TODO: Fix this so it actually does something
			var rotation = head.RotationDegrees;
			rotation.x = Mathf.Clamp(rotation.x, -79, 79);
			head.RotationDegrees = rotation;

			mouseMovement = Vector2.Zero;
		}
	}

	private void walk(float delta)
	{		
		direction = Vector3.Zero;

		// Direction player is looking
		aim = head.GetGlobalTransform().basis;
		
		// Get input
		if (Input.IsActionPressed("Forward"))
			direction.z -= 1;
		if (Input.IsActionPressed("Backward"))
			direction.z += 1;
		if (Input.IsActionPressed("Left"))
			direction.x -= 1;
		if (Input.IsActionPressed("Right"))
			direction.x += 1; 

		// Rotate direction
		direction = direction.Rotated(Vector3.Up, head.Rotation.y);

		direction.Normalized();

		// Determine if player is on the ground and do some gravity stuff
		if (IsOnFloor())
		{
			onGround = true;
			var normal = onGroundRaycast.GetCollisionNormal();
			var floorAngle = Math.PI*(Math.Acos(normal.Dot(Vector3.Up)));
			if (floorAngle > maxSlopAngle)
				velocity.y += gravity * delta;
		}
		else
		{
			if (!onGroundRaycast.IsColliding())
				onGround = false;
			velocity.y += gravity;
		}

		// Jump
		if (Input.IsActionPressed("Jump") && onGround)
		{
			velocity.y += jumpHeight; 
			onGround = false;
		}

		Vector3 horizVelocity = velocity;
		horizVelocity.y = 0;

		// Speed to move at
		float speed = maxWalkSpeed;

		// Check if also running
		if (Input.IsActionPressed("Run"))
			speed = 6;

		// Position player would move to at max speed
		var target = direction * speed;

		// Decide if accel or deaccel
		float acceleration;
		if (direction.Dot(horizVelocity) > 0)
			acceleration = accel;
		else 
			acceleration = deAccel;

		// Calculate amount to move based on acceleration.
		horizVelocity = horizVelocity.LinearInterpolate(target, accel * delta);
				
		velocity.x = horizVelocity.x;
		velocity.z = horizVelocity.z;

		// Move only when movement is not extremely small. Workaround for some godot bug
		if (velocity.Length() > 0.1f)
			// TODO: Velocity is not supposed to be multiplied by delta but it is above. Removing delta messes up velocity though so this needs experimenting.
			velocity = MoveAndSlide(velocity, Vector3.Up, false, 4, 0.784398f, false);

		// Apply impulse to rigidbodies player is colliding with
		for (int i = 0; i < GetSlideCount(); i++)
		{
        	var collision = GetSlideCollision(i);

			// TODO: Replace this with something more proper than try catch
			try 
			{
			((RigidBody)collision.Collider).ApplyCentralImpulse(-collision.Normal + -collision.Normal * Velocity.Length() );
			}
			catch {}
		}
	}

	private void fly(float delta)
	{		
		direction = Vector3.Zero;

		// Direction player is looking
		var aim = head.GetGlobalTransform().basis;
		
		// Get input
		if (Input.IsActionPressed("Forward"))
			direction -= aim.z;
		if (Input.IsActionPressed("Backward"))
			direction += aim.z;
		if (Input.IsActionPressed("Left"))
			direction -= aim.x;
		if (Input.IsActionPressed("Right"))
			direction += aim.x; 

		direction.Normalized();

		// Position player would move to at max speed
		var target = direction * flySpeed;

		// Calculate amount to move based on acceleration
		velocity = velocity.LinearInterpolate(target, flyAccel * delta);
				
		// Move
		MoveAndSlide(velocity * delta);
	}
	
	private void shoot()
	{		
		// Start timer/cooldown
		firingTimer.Start();

		// Play firing effects once
		//((Particles)gunParticles).Emitting = true;

		// Play firing sound
		firingSound.Play();

		// Spawn new bullet instance
		Node b = bullet.Instance();
		// Set current position and the direction it should travel
		((Player_Bullet)b).SetPosAndHeading(firingPosition.GetGlobalTransform().origin, -aim[2].Normalized());
		// Set parent
		((Player_Bullet)b).Parent = this;
		// Add it to the bullet container
		bulletContainer.AddChild(b);

		ammo--;
	}

		public override void _Process(float delta)
    {
		look();

		// Update text on HUD
		HealthLabel.Text = Health.ToString();
		AmmoLabel.Text = ammo.ToString();
    }
	
	public override void _PhysicsProcess(float delta)
    {
		if (flying)
			fly(delta);
        else 
			walk(delta);
			
		if (Input.IsActionPressed("Shoot") && ammo > 0)
		{
			if (firingTimer.IsStopped())
				shoot();
		}

		// Chek if player tries to interact using E
		if (Input.IsActionJustPressed("Interact")) 
			_Interact();
		
		// Check if game should exit
		if (Input.IsActionPressed("ui_cancel"))
			GetTree().Quit();
    }
	
	private void OnLadderAreaEntered(object body)
	{
		if (((Node)body).Name == "Player")
		{
		flying = true;
		}
	}
	
	private void OnLadderAreaExited(object body)
	{
		if (((Node)body).Name == "Player")
		{
		flying = false;
		}
	}


	// Function for taking damage
	public void Hit()
	{
		// Apply damage to health and keep it within the interval of 0 and max Health.
		Health = Mathf.Clamp((Health - 5), 0, MaxHealth);

		// Play hit sound
		hitSound.Play();

		// Play animation
		((AnimationPlayer)GetNode("HUD/AnimationPlayer")).Play("Damage");

		if (Health == 0) 
		{
			Kill();
		}
	}
	public void Kill()
	{
		GetTree().ChangeScene("res://Scenes/Game Over.tscn");
	}
	public void AddAmmo(int amount)
	{
		ammo += amount;
		addAmmoSound.Play();
		// Play animation
		((AnimationPlayer)GetNode("HUD/AnimationPlayer")).Play("Pick Up");
	}
	public void AddHealth(int amount)
	{
		Health = Mathf.Clamp(Health += amount, 0, 100);
		addHealthSound.Play();
		// Play animation
		((AnimationPlayer)GetNode("HUD/AnimationPlayer")).Play("Pick Up");
	}
}