using Godot;
using System;

public class Button : Godot.Button
{
    [Export] private string sceneToLoad;
    [Export] private int TabToLoad;
    private bool exitButton;
    [Export] private bool sceneButton = false;
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
        
        if (sceneButton)
            GetTree().ChangeScene("res://Scenes/" + sceneToLoad.ToString() + ".tscn");
        else
        {
            var container = ((TabContainer)GetNode("/root/Control/TabContainer"));
            container.CurrentTab = TabToLoad;
        }

    }
}
