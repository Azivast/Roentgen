using Godot;
using System;

public class Door : StaticBody
{
    private AnimationPlayer animation;
    private AudioStreamPlayer3D openAudio;
    
    private AudioStreamPlayer3D closeAudio;

    private int state = 0;
    private const int closed = 0;
    private const int open = 1;

    // Timer used to close the door after a while
    private Timer Timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        openAudio = (AudioStreamPlayer3D)GetNode("Audio open");
        closeAudio = (AudioStreamPlayer3D)GetNode("Audio close");
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
			animation.PlayBackwards("Open");
            closeAudio.Play();
        } 
    }

    // Method that runs when door is interacted with
    public void Interact()
  	{
	    if (state == closed)
        {
            state = open;
            Timer.Start();
            animation.Play("Open");
            openAudio.Play();
        }
  	}	
}
