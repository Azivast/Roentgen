using Godot;
using System;

public class HealthPack : Sprite3D
{
    public void OnAreaBodyEntered(Node body)
    {
        if (body.Name == "Player")
        {
            ((Player)body).AddHealth(50);
            QueueFree();
        }
    }
}
