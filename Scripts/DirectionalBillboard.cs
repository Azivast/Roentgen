using Godot;
using System;

public class DirectionalBillboard : Sprite3D
{
    // Animation colum
    [Export] int colum;

    Head camera;

    public void SetCamera(Head camera)
    {
        this.camera = camera;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (camera is null) 
        {
            //GD.Print("Camera is null");
            return;
        }

        // TODO: Explain all this jazz with some comments
        var player_forward = (GlobalTransform.origin - camera.GlobalTransform.origin).Normalized();
        var forward = GlobalTransform.basis.z;
        var left = GlobalTransform.basis.x;

        float left_dot = left.Dot(player_forward);
        float forward_dot = forward.Dot(player_forward);

        int row = 0;
        FlipH = false;

        if (forward_dot < -0.85)
            row = 0; // front sprite

        else if (forward_dot > 0.85)
            row = 4; // back sprite
            
        else
        {
            FlipH = left_dot > 0;

            if (Mathf.Abs(forward_dot) < 0.3)
                row = 2; // left sprite
            else if (forward_dot < 0)
                row = 1; // forward left sprite
            else
                row = 3; // back left sprite
        }
        Frame = colum + row * 4;
    }
}
