using Godot;
using System;

public class Door : StaticBody
{
    private PathFollow path;
    private float movementSpeed = 1f;

    public bool Closed = true;
    // Timer used to close the door after a while
    private Timer Timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get path
        path = (PathFollow)GetParent();
        Timer = (Timer)GetNode("Timer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // Open the door if it is unlocked
        if (Closed == false) 
        {
            path.Offset += movementSpeed*delta;
            Console.WriteLine(path.Offset);

            // Close the door once the timer runs out
            if (Timer.IsStopped()) Closed = true;
        }

        // Otherwise close it
        else path.Offset -= movementSpeed*delta; // TODO: Rework code to not update path unless explicitly told to do so
    }

    // Method that runs when interacted with
    public void Interact()
  	{
        Closed = false;
        Timer.Start();
  	}	
}
