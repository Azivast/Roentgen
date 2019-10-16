using Godot;
using System;

public class Player : KinematicBody
{
	// Movement variables
	Vector3 direction;
	Vector3 velocity;

	// Mouse variables
	[Export] public static float mouseSensitivity = 0.003f;
	private Vector2 mouseMovement;

	// Fly variables
	private float flySpeed = 40f;
	private float flyAccel = 1f;
	private bool flying = false;

	// Walk variables
	private float gravity = -9.82f * 0.02f;
	[Export] public float maxWalkSpeed = 4f;
	[Export] private float maxSprintSpeed = 7f;
	[Export] private float accel = 2f;
	[Export] private float deAccel = 10f;

	// Jumping variables
	[Export] private float jumpHeight = 6;
	private bool onGround = false;
	private RayCast onGroundRaycast;

	// Slope variables
	[Export] private int maxSlopAngle = 25;

  // Shooting variables
	private RayCast raycast;
	[Export] private Node gunParticles;
	[Export] private PackedScene bullet;
	private Node bulletContainer;

	private Spatial head;



  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
	{
		raycast = (RayCast)GetNode("Head/Camera/RayCast");
		onGroundRaycast = (RayCast)GetNode("Head/Camera/RayCast");
		head =  (Spatial)GetNode("Head");
		bulletContainer = GetNode("Bullet Container");
  }
	
  public override void _Input(InputEvent @event)
  {
		// Rotate
		if (@event is InputEventMouseMotion)
    	{
			mouseMovement = ((InputEventMouseMotion)@event).Relative;
		}
  }	

	private void look()
	{
		if (mouseMovement.Length() != 0)
		{
			head.Rotate(Vector3.Up, -mouseMovement.x * mouseSensitivity);
			head.RotateObjectLocal(Vector3.Left, mouseMovement.y * Player.mouseSensitivity);

			mouseMovement = Vector2.Zero;
		}
	}

	private void walk(float delta)
	{		
		direction = Vector3.Zero;

		// Direction player is looking
		var aim = head.GetGlobalTransform().basis;
		
		// Get input
		if (Input.IsActionPressed("move_forward"))
			direction.z -= 1;
		if (Input.IsActionPressed("move_backwards"))
			direction.z += 1;
		if (Input.IsActionPressed("move_left"))
			direction.x -= 1;
		if (Input.IsActionPressed("move_right"))
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
		if (Input.IsActionPressed("move_jump") && onGround)
		{
			velocity.y += jumpHeight; 
			onGround = false;
		}

		Vector3 horizVelocity = velocity;
		horizVelocity.y = 0;

		// Speed to move at
		float speed;
		if (Input.IsActionPressed("move_sprint"))
			speed = maxSprintSpeed;
		else 
			speed = maxWalkSpeed;

		// Position player would move to at max speed
		var target = direction * speed;

		// Decide of accel or deaccel
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
			velocity = MoveAndSlide(velocity, Vector3.Up);
	}

	private void fly(float delta)
	{		
		direction = Vector3.Zero;

		// Direction player is looking
		var aim = head.GetGlobalTransform().basis;
		
		// Get input
		if (Input.IsActionPressed("move_forward"))
			direction -= aim.z;
		if (Input.IsActionPressed("move_backwards"))
			direction += aim.z;
		if (Input.IsActionPressed("move_left"))
			direction -= aim.x;
		if (Input.IsActionPressed("move_right"))
			direction += aim.x; 

		direction.Normalized();

		// Position player would move to at max speed
		var target = direction * flySpeed;

		// Calculate amount to move based on acceleration.
		velocity = velocity.LinearInterpolate(target, flyAccel * delta);
				
		// Move
		MoveAndSlide(velocity * delta);
	}
	
	private void shoot()
	{		
		// Play firing effects once
		//((Particles)gunParticles).Emitting = true;

		// Spawn new bullet instance
		Node b = bullet.Instance();
		((Node)b).SetHead(new Vector3(1, 0, 0));
		b.GetChild("Player Bullet").Heading 
		bulletContainer.AddChild(b);
	}
	
	public override void _Process(float delta)
    {
		look();
		if (flying)
			fly(delta);
        else 
			walk(delta);

		if (Input.IsActionPressed("shoot"))
		{
			shoot();
		}
		
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
}