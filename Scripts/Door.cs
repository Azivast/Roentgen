using Godot;
using System;

public class Door : StaticBody
{
    private AnimationPlayer animation;
    private bool animating = false;

    private int state = 0;
    private const int closed = 0;
    private const int open = 1;

    // Timer used to close the door after a while
    private Timer Timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animation = (AnimationPlayer)GetNode("AnimationPlayer");
        Timer = (Timer)GetNode("Timer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // Close the door once the timer runs out
        if (state == open && Timer.IsStopped()) 
        {
            state = closed;
            animating = true;
			animation.PlayBackwards("Open");
        } 
        
    }

    // Method that runs when door is interacted with
    public void Interact()
  	{
        // Open the door if it is unlocked
        if (animating == false) 
        {
		    if (state == closed)
            {
                state = open;
                Timer.Start();
                animation.Play("Open");
            }
        }
  	}	
}
