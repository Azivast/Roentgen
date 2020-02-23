using Godot;
using System;

public class Options2 : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     SoundSlider();
     MusicSlider();
 }

    public void ToggleFullscreen()
    {
        OS.SetWindowFullscreen(!OS.IsWindowFullscreen()); 
    }

    public void MusicSlider()
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), ((HSlider)GetNode("VBoxContainer/HBoxContainer2/HSlider")).GetValue());
    }
    public void SoundSlider()
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Sounds"), ((HSlider)GetNode("VBoxContainer/HBoxContainer4/HSlider")).GetValue());
    }

    public void SoundMute()
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), !AudioServer.IsBusMute(AudioServer.GetBusIndex("Master")));
    }

    public void MusicMute()
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), !AudioServer.IsBusMute(AudioServer.GetBusIndex("Music")));
    }
}
