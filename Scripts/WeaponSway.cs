using Godot;
using System;

public class WeaponSway : Spatial
{
    [Export] private float amount = 0.5f;
    [Export] private float smoothAmount = 10f;  
    [Export] private float maxSway = 15f; 

    private  Vector3 initialPosition;

    float mouseMovementX;
    float mouseMovementY;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        initialPosition = Transform.origin;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            mouseMovementX = ((InputEventMouseMotion)@event).Relative.x * amount;
            mouseMovementY = ((InputEventMouseMotion)@event).Relative.y * amount;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        float movementX = (mouseMovementY/5) - RotationDegrees.x * smoothAmount * delta;
        float movementY = (mouseMovementX/5) - RotationDegrees.y * smoothAmount * delta;

        Vector3 movement = new Vector3(movementX, movementY, 0);
        Vector3 finalMovement = RotationDegrees + movement;

        // Limit final movement
        finalMovement.x = Mathf.Clamp(finalMovement.x, -maxSway, maxSway);
        finalMovement.y = Mathf.Clamp(finalMovement.y, -maxSway, maxSway);

        

        // Rotate
        SetRotationDegrees(finalMovement);
    }
}