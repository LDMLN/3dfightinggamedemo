using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class Fireball : Node3D
{
    const int fireballSpeed = 4;
    Vector3 directionToEnemy;

    public override void _Process(double delta)
    {
        MoveTowardsTarget(delta);
    }

    private void MoveTowardsTarget(double delta)
    {
        GlobalPosition += directionToEnemy.Normalized() * fireballSpeed * (float)delta;
    }

    public void SetDirectionToEnemy(Vector3 directionToFollow)
    {
        directionToEnemy = directionToFollow;
        StartSelfFreeTimer();
    }

    private async Task StartSelfFreeTimer()
    {
        await ToSignal(GetTree().CreateTimer(10), "timeout");
        QueueFree();
    }
}
