using Godot;
using System;

public class Head : Spatial
{
/*     [Export] private float amount;
    [Export] private float maxAmount;
    [Export] private float smoothAmount;  

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
            float movementX = ((InputEventMouseButton)@event).Relative.x * amount;
            float movementY = ((InputEventMouseButton)@event).Relative.y * amount;
            float movementY = @event. * amount;
            float movementY = test.Relative.y *amount;
        }
        if (@event is InputEventMouseButton)
        {GD.Print("Mouse ", ((InputEventMouseButton)@event).Position);}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        Vector3 finalposition = new Vector3(movementX, movementY, 0);
        //SetTranslation(Vector3.Lerp(Transform.origin, finalposition + initialPosition, delta * smoothAmount));
        //Transform.origin.LinearInterpolate(finalposition + initialPosition, delta * smoothAmount);
    } */
}
