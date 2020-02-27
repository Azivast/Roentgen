using Godot;
using System;

public class WinButton : StaticBody
{
    [Export] string LoadScene = "Win";


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }


    // Method that runs when interacted with
    public void Interact()
    {
        //GetTree().ChangeScene("res://Scenes/" + LoadScene + ".tscn");
        GetTree().ChangeScene("res://Scenes/Win.tscn");
    }	
}
