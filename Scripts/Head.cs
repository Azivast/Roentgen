using Godot;
using System;

public class Head : Spatial
{
    Vector3 velocity;
    // Script in the kinematic body "Player"
    Player player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = (Player)GetParent();
        GetTree().CallGroup("EnemySprites", "SetCamera", this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {   

    }  
}
