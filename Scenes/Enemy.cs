using Godot;
using System;

public class Enemy : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private bool seesPlayer = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //
        seesPlayer = false;
    }

    private void OnBodyEntered(Node body)
	{
		if (body.Name == "Player")
        {
            seesPlayer = true;
        }
	}
}
