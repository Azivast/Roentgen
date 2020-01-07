using Godot;
using System;

public class Enemy_Bullet : Area
{
    [Export] private float speed = 0.05f;
    private Vector3 velocity = new Vector3();
    // Direction of travel
    private Vector3 heading;

    public override void _Process(float delta)
    {
        // Move bullet
        SetTranslation(GetTranslation() + heading);
    }

    public void SetPosAndHeading(Vector3 position, Vector3 heading)
    {
        SetTranslation(position);
        heading = heading.Normalized();
        this.heading = heading*speed;
    }

    private void OnLifeTimeTimeout()
    {
        //RemoveBullet();
    }

    private void OnBodyEntered(Node body)
    {
        if (body.Name == "Enemy") {}

        // Damage player if it hits them
        else if (body.HasMethod("Damage"))
        {
            ((Player)body).Hit();
            RemoveBullet();
        }

        else
            RemoveBullet();
    }

    private void RemoveBullet()
    {
        // Safely remove node
        //QueueFree();
    }
}
