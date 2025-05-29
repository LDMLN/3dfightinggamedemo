using Godot;
using System;

public partial class DefendState : NodeState
{
    [Export] public float DefendDuration = 2f;
    [Export] public float BackpedalSpeed = 2f;

    private float _defendTimer = 0f;
    private AnimationPlayer _animationPlayer;
    private bool _isBlocking = false;

    public override void _OnEnter()
    {
        GD.Print("Entering DefendState");

        _defendTimer = 0f;
        _isBlocking = true;

        _animationPlayer = _owner.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (_animationPlayer != null)
        {
            _animationPlayer.Play("Block");
        }
    }

    public override void _OnPhysicsProcess(double delta)
    {
        if (_target == null || _owner == null) return;

        _defendTimer += (float)delta;

        // Move backward away from the player
        Vector3 toTarget = (_target.GlobalTransform.Origin - _owner.GlobalTransform.Origin).Normalized();
        Vector3 backDirection = -toTarget;
        _owner.GlobalTransform = _owner.GlobalTransform.Translated(backDirection * BackpedalSpeed * (float)delta);
    }

    public override void _OnNextTransitions()
    {
        if (_target == null || _owner == null)
            return;

        float distanceToPlayer = _owner.GlobalTransform.Origin.DistanceTo(_target.GlobalTransform.Origin);

        if (_defendTimer >= DefendDuration)
        {
            if (distanceToPlayer > 5f)
            {
                EmitSignal(SignalName.Transition, "MoveToPlayer");
            }
            else
            {
                EmitSignal(SignalName.Transition, "AttackState");
            }
        }
    }

    public override void _OnExit()
    {
        GD.Print("Exiting DefendState");

        if (_animationPlayer != null && _isBlocking)
        {
            _animationPlayer.Stop();
            _isBlocking = false;
        }
    }
}
