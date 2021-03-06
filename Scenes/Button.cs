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
        if (sceneButton)
        {
            if (exitButton) GetTree().Quit();
            //else if (sceneToLoad == "Level1") GetTree().ChangeSceneTo(ResourceLoader.Load<PackedScene>("res://Scenes/Level1.tscn"));
            else GetTree().ChangeScene("res://Scenes/" + sceneToLoad.ToString() + ".tscn");
            
        }
        
        else
        {
            try
            {
                var container = ((TabContainer)GetNode("/root/Control/TabContainer"));
                container.CurrentTab = TabToLoad;
            }
            catch{}
        }

    }
}
