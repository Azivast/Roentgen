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
	public bool IsDead = false;

	// HUD variables
	private Label HealthLabel;
	private Label AmmoLabel;
	private Label interactLabel;

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
	public float maxWalkSpeed = 4f;
	private float maxSprintSpeed = 4f;
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
			interactLabel = (Label)GetNode("HUD/Interact Help");
			
			// Set life to max life
			Health = MaxHealth;

			// Set Player variable for all enemies
			GetTree().CallGroup("Enemies", "SetPlayer", this);

			// Capture mouse
			Input.SetMouseMode(Input.MouseMode.Captured);
		}
	
	public override void _Input(InputEvent @event)
	{
			// Rotate
			if (@event is InputEventMouseMotion)
			{
				mouseMovement = ((InputEventMouseMotion)@event).Relative;
			}
	}	

	private void Interact()
	{
		var collider = raycast.GetCollider();
	
		interactLabel.Visible = false;

		if (collider != null)
		{
			if (collider.HasMethod("Interact")) 
			{
				interactLabel.Visible = true;
				if (Input.IsActionJustPressed("Interact")) 
					collider.Call("Interact");
			}
		}
	}	

	private void Look()
	{
		if (mouseMovement.Length() != 0)
		{
			float xRotation = -mouseMovement.y * mouseSensitivity;
			float yRotation = -mouseMovement.x * mouseSensitivity;
			float xRotationFinal = xRotation + head.Rotation.x;
			float yRotationFinal = yRotation + head.Rotation.y;

			if (xRotation > Math.PI/2)
				xRotationFinal = (float)Math.PI/2;
			if (xRotation < -Math.PI/2)
				xRotationFinal = -(float)Math.PI/2;

			if (xRotationFinal > Math.PI/2)
				xRotationFinal = (float)Math.PI/2;
			if (xRotationFinal < -Math.PI/2)
				xRotationFinal = -(float)Math.PI/2;

			head.SetRotation(new Vector3(xRotationFinal, yRotationFinal, 0));	

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
			speed = maxSprintSpeed;

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
	
	private void Shoot()
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
		// Check if game should exit
		if (Input.IsActionPressed("ui_cancel"))
			GetTree().ChangeScene("res://Scenes/Main menu.tscn");

		// Update text on HUD
		HealthLabel.Text = Health.ToString();
		AmmoLabel.Text = ammo.ToString();

		// Don't do anything if else player is dead
		if (IsDead) return;

		Look();
		Interact();
    }
	
	public override void _PhysicsProcess(float delta)
    {
		// Don't do anything if player is dead
		if (IsDead) return;


		if (flying)
			fly(delta);
        else 
			walk(delta);
			
		if (Input.IsActionPressed("Shoot") && ammo > 0)
		{
			if (firingTimer.IsStopped())
				Shoot();
		}
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
		IsDead = true;

		// Hide some nodes
		((Control)GetNode("HUD")).Visible = false;
		((WeaponSway)GetNode("Head/Gun")).Visible = false;
		
		// Activate collision shape
		CollisionShape collisionShape = (CollisionShape)GetNode("RigidBody/CollisionShape");
		collisionShape.Disabled = false;
		// Reparent camera to rigibody
		this.RemoveChild(head);
		RigidBody rigidBody = (RigidBody)GetNode("RigidBody");
		rigidBody.AddChild(head);
		head.SetOwner(rigidBody);

		// Show game over screen
		((Control)GetNode("Game Over")).Visible = true;

		// Show mouse cursor
		Input.SetMouseMode(Input.MouseMode.Visible);
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