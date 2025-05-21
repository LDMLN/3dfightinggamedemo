using Godot;
using System;

public partial class MoveToPlayerState : NodeState
{
    [Export]
    public NodePath TargetPath; // Path to the player node

    [Export]
    public NodePath AgentPath; // Path to the NavigationAgent3D node

    [Export]
    public float Speed = 3.0f; // Movement speed of the enemy

    [Export]
    public float Gravity = 9.8f; // Gravity applied to the enemy

    [Export]
    public float AttackRange = 2.0f; // Distance at which the enemy transitions to attack

    private Node3D _target; // Reference to the player node
    private NavigationAgent3D _agent; // Reference to the navigation agent
    private CharacterBody3D _owner; // Reference to the enemy's character body

    private Vector3 _velocity = Vector3.Zero; // Current velocity of the enemy

    public override void _Ready()
    {
        base._Ready();

        // Initialize references to the target, agent, and owner
        _target = GetNode<Node3D>(TargetPath);
        _agent = GetNode<NavigationAgent3D>(AgentPath);
        _owner = GetOwner<CharacterBody3D>(); // Assumes the parent node is the enemy character
    }

    public override void _OnEnter()
    {
        GD.Print("Entering MoveToPlayerState");

        // Set the initial target position for the navigation agent
        if (_agent != null && _target != null)
        {
            _agent.TargetPosition = _target.GlobalTransform.Origin;
        }
    }

    public override void _OnPhysicsProcess(double delta)
    {
        if (_agent == null || _owner == null || _target == null)
            return;

        // Apply gravity if the enemy is not on the floor
        if (!_owner.IsOnFloor())
        {
            _velocity.Y -= Gravity * (float)delta;
        }
        else
        {
            _velocity.Y = 0;
        }

        // Update the target position for the navigation agent
        _agent.TargetPosition = _target.GlobalTransform.Origin;

        // Get the next position in the path
        Vector3 nextPathPos = _agent.GetNextPathPosition();
        Vector3 direction = (nextPathPos - _owner.GlobalTransform.Origin).Normalized();

        // Calculate horizontal velocity
        Vector3 horizontalVelocity = direction * Speed;
        _velocity.X = horizontalVelocity.X;
        _velocity.Z = horizontalVelocity.Z;

        // Move the enemy
        _owner.Velocity = _velocity;
        _owner.MoveAndSlide();

        // Rotate the enemy to face the player
        if (direction.Length() > 0.1f)
            _owner.LookAt(_target.GlobalTransform.Origin, Vector3.Up);
    }

    public override void _OnNextTransitions()
    {
        if (_owner == null || _target == null)
            return;

        // Check if the enemy is within attack range of the player
        float distanceToPlayer = _owner.GlobalTransform.Origin.DistanceTo(_target.GlobalTransform.Origin);
        if (distanceToPlayer <= AttackRange)
        {
            // Emit a signal to transition to the "attack" state
            EmitSignal(SignalName.Transition, "attack");
        }
    }

    public override void _OnExit()
    {
        GD.Print("Exiting MoveToPlayerState");
    }
}
