using Godot;
using System;

public class Button : Godot.Button
{
    [Export] private string sceneToLoad;
    private bool exitButton;
    public override void _Ready()
    {
        if (sceneToLoad == "exit")
        {
            exitButton = true;
        }
    }

    public void OnButtonPressed()
    {
        if (exitButton)
            GetTree().Quit();
        
        else
            GetTree().ChangeScene("res://Scenes/" + sceneToLoad.ToString() + ".tscn");
    }
}
