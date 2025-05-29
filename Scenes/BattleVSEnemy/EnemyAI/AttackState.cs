using Godot;
using System;

public partial class AttackState : NodeState
{
    [Export] public float AttackCooldown = 1.5f;

    private float _cooldownTimer = 0f;
    private int _hitCounter = 0;
    private bool _isAttacking = false;

    private string[] _attackOptions = new string[] {
        "Jab Punch",
        "Hook Punch",
        "Kick",
        "Roundhouse Kick"
    };

    private Random _rand = new Random();

    private AnimationPlayer _animationPlayer;

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

        _animationPlayer = _owner.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (_animationPlayer != null)
            _animationPlayer.AnimationFinished += OnAnimationFinished;

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
    if (_target == null || _owner == null)
        return;

    float distanceToPlayer = _owner.GlobalTransform.Origin.DistanceTo(_target.GlobalTransform.Origin);

    if (distanceToPlayer > 5f)
    {
        EmitSignal(SignalName.Transition, "MoveToPlayer"); // corrected
    }
    else if (_hitCounter >= 2)
    {
        EmitSignal(SignalName.Transition, "DefendState"); // corrected
    }
    else if (_cooldownTimer >= AttackCooldown)
    {
        EmitSignal(SignalName.Transition, "MoveToPlayer"); // instead of "move", clarify with your actual node
    }
}


    public override void _OnExit()
    {
        GD.Print("Exiting AttackState");

        if (_animationPlayer != null)
            _animationPlayer.AnimationFinished -= OnAnimationFinished;

        _isAttacking = false;
    }

    private void PlayRandomAttack()
    {
        if (_owner == null || _animationPlayer == null || _isAttacking)
            return;

        var animationTree = _owner.GetNodeOrNull<AnimationTree>("AnimationTree");
        if (animationTree == null)
        {
            GD.PrintErr("Missing AnimationTree on enemy.");
            return;
        }

        string attack = _attackOptions[_rand.Next(_attackOptions.Length)];
        GD.Print("Enemy performing attack: " + attack);

        var stateMachine = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Attack");
        animationTree.Set("parameters/StateMachine/current", "Attack");
        animationTree.Set("parameters/Attack/parameters/BlendPosition", attack);
        _animationPlayer.Play(attack);

        _isAttacking = true;
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (_owner == null || !_owner.IsInsideTree()) return;
        if (!_isAttacking) return;

        _isAttacking = false;

        // Check transition conditions again to avoid attacking when transitioning
        if (_hitCounter < 2 && _cooldownTimer < AttackCooldown)
        {
            PlayRandomAttack();
        }
    }
}
