using Godot;
using System;

public class EnemySelect : Control
{
    public void Human()
    {
        ((Global)GetNode("/root/Global")).HumanEnemies = true;
        GetTree().ChangeScene("res://Scenes/Main menu.tscn");

    }
    public void Wasp()
    {
        ((Global)GetNode("/root/Global")).HumanEnemies = false;
        GetTree().ChangeScene("res://Scenes/Main menu.tscn");
    }
}
