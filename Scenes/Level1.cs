using Godot;
using System;

public class Level1 : Spatial
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent @event)
    {
//		// Free mouse
//		if (Input.IsActionJustPressed("free_mouse"))
//        {
//		    if (Input.GetMouseMode() == Input.MouseMode.Captured)
//                Input.SetMouseMode(Input.MouseMode.Visible);
//            else
//                Input.SetMouseMode(Input.MouseMode.Captured);
//		}
    }	

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }
}
