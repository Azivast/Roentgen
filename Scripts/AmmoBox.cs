using Godot;
using System;

public class AmmoBox : Sprite3D
{
    public void OnAreaBodyEntered(Node body)
    {
        if (body.Name == "Player")
        {
            ((Player)body).AddAmmo(5);
            QueueFree();
        }
    }
}
