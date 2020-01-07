using Godot;
using System;

public class Camera : Godot.Camera
{
    float swayAmount = 0.3f;
    float smoothAmount = 10f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        float sidewaysMovement = 0;

        if (Input.IsActionPressed("move_left"))
            sidewaysMovement = 1;
        if (Input.IsActionPressed("move_right"))
            sidewaysMovement = -1; 

        sidewaysMovement *= swayAmount;

        float rotation = (sidewaysMovement/1) - RotationDegrees.z * smoothAmount * delta;

        Vector3 finalRotation= new Vector3(0, 0, rotation);

        // Rotate
        SetRotationDegrees(RotationDegrees + finalRotation);
    }
}
