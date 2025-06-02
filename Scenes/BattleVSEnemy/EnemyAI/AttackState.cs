using Godot;
using System;

public partial class AttackState : NodeState
{
    [Export] public float AttackCooldown = 1.5f;

    private float _cooldownTimer = 0f;
    private int _hitCounter = 0;
    private bool _isAttacking = false;

    // Confirmed attack animation names (from Locomotion state machine)
    private readonly string[] _attackAnimations = new string[]
    {
        "Jab Punch",
        "Hook Punch",
        "Kick",
        "Roundhouse Kick"
    };

    private Random _rand = new Random();

    private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _stateMachine;

    public void OnHit()
    {
        _hitCounter++;
    }

    public override void _OnEnter()
    {
        GD.Print("Entering AttackState");

        _cooldownTimer = 0f;
        _hitCounter = 0;
        _isAttacking = false;

        _animationTree = _owner.GetNodeOrNull<AnimationTree>("AnimationTree");

        if (_animationTree == null)
        {
            GD.PrintErr("AttackState: AnimationTree not found.");
            return;
        }

        _stateMachine = _animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        if (_stateMachine == null)
        {
            GD.PrintErr("AttackState: Failed to retrieve AnimationNodeStateMachinePlayback.");
            return;
        }

        _animationTree.Active = true;

        PlayRandomAttack();
    }

    public override void _OnPhysicsProcess(double delta)
    {
        if (_target == null || _owner == null) return;

        _cooldownTimer += (float)delta;

        // Face the player
        Vector3 toTarget = (_target.GlobalTransform.Origin - _owner.GlobalTransform.Origin).Normalized();
        if (toTarget.Length() > 0.1f)
            _owner.LookAt(_target.GlobalTransform.Origin, Vector3.Up);
    }

    public override void _OnNextTransitions()
    {
        if (_target == null || _owner == null) return;

        float distanceToPlayer = _owner.GlobalTransform.Origin.DistanceTo(_target.GlobalTransform.Origin);

        if (distanceToPlayer > 2f)
        {
            EmitSignal(SignalName.Transition, "MoveToPlayer");
        }
        else if (_hitCounter >= 2)
        {
            EmitSignal(SignalName.Transition, "DefendState");
        }
        else if (_cooldownTimer >= AttackCooldown)
        {
            EmitSignal(SignalName.Transition, "MoveToPlayer");
        }
    }

    public override void _OnExit()
    {
        GD.Print("Exiting AttackState");
        _isAttacking = false;
    }

    private void PlayRandomAttack()
    {
        if (_owner == null || _animationTree == null || _stateMachine == null || _isAttacking)
            return;

        string selectedAttack = _attackAnimations[_rand.Next(_attackAnimations.Length)];
        GD.Print($"Enemy performing attack: {selectedAttack}");

        _stateMachine.Travel(selectedAttack);
        _isAttacking = true;

        // Optionally: set a timer or manually reset _isAttacking based on animation length
    }
}
