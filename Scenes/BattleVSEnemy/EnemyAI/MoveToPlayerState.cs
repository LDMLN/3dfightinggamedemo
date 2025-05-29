using Godot;
using System;

public partial class MoveToPlayerState : NodeState
{
    [Export]
    public float Speed = 3.0f;
    [Export]
    public float Gravity = 9.8f;
    [Export]
    public float AttackRange = 2.0f;

    private Vector3 _velocity = Vector3.Zero;

    public override void _OnEnter()
    {
        GD.Print("Entering MoveToPlayerState");
        if (_agent != null && _target != null)
        {
            _agent.TargetPosition = _target.GlobalTransform.Origin;
        }
    }

    public override void _OnPhysicsProcess(double delta)
    {
        if (_agent == null || _owner == null || _target == null) return;

        if (!_owner.IsOnFloor())
            _velocity.Y -= Gravity * (float)delta;
        else
            _velocity.Y = 0;

        _agent.TargetPosition = _target.GlobalTransform.Origin;
        Vector3 nextPathPos = _agent.GetNextPathPosition();
        Vector3 direction = (nextPathPos - _owner.GlobalTransform.Origin).Normalized();

        Vector3 horizontalVelocity = direction * Speed;
        _velocity.X = horizontalVelocity.X;
        _velocity.Z = horizontalVelocity.Z;

        _owner.Velocity = _velocity;
        _owner.MoveAndSlide();

        if (direction.Length() > 0.1f)
            _owner.LookAt(_target.GlobalTransform.Origin, Vector3.Up);

        GD.Print($"Distance to target: {_owner.GlobalTransform.Origin.DistanceTo(_target.GlobalTransform.Origin)}");
    }

    public override void _OnNextTransitions()
    {
        float distanceToPlayer = _owner.GlobalTransform.Origin.DistanceTo(_target.GlobalTransform.Origin);
        if (distanceToPlayer <= AttackRange)
        {
            EmitSignal(SignalName.Transition, "attack");
        }
    }

    public override void _OnExit()
    {
        GD.Print("Exiting MoveToPlayerState");
    }
}
