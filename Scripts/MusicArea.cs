using Godot;
using System;

public class MusicArea : Area
{
    AudioStreamPlayer music1;
    AudioStreamPlayer music2;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        music1 = (AudioStreamPlayer)GetNode("../Music");
        music2 = (AudioStreamPlayer)GetNode("../Music2");
    }

    public void ChangeMusic(Node body)
    {
        if (body.Name == "Player")
        {
            music1.Stop();
            music2.Play();
        }
    }
}
