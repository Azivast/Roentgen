using Godot;
using System;

public class WeaponSway : Spatial
{
    [Export] private float amount = 10;
    [Export] private float maxAmount = 100;
    [Export] private float smoothAmount = 1;  

    private  Vector3 initialPosition;

    float movementX;
    float movementY;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //initialPosition = Transform.origin;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            float movementX = ((InputEventMouseMotion)@event).Relative.x * amount;
            float movementY = ((InputEventMouseMotion)@event).Relative.y * amount;
            
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        initialPosition = Translation;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);
        Vector3 finalposition = new Vector3(movementX, movementY, 0);
        Console.WriteLine(movementX);
        Console.WriteLine(movementY);
        //SetTranslation(Vector3.Lerp(Transform.origin, finalposition + initialPosition, delta * smoothAmount));
        Translation.LinearInterpolate(finalposition + initialPosition, delta * smoothAmount);
    }
}
