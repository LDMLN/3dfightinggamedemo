using Godot;
using System;

public partial class NodeState : Node
{
    [Signal]
    public delegate void TransitionEventHandler(string nextState);
    protected Node3D _target;
    protected NavigationAgent3D _agent;
    protected CharacterBody3D _owner;

    public virtual void SetTarget(Node3D target)
    {
        _target = target;
    }

    public virtual void SetAgent(NavigationAgent3D agent)
    {
        _agent = agent;
    }

    public override void _Ready()
    {
        _owner = GetOwner<CharacterBody3D>();
    }


    public virtual void _OnProcess(double delta)
    {
        // Override in subclasses
    }

    public virtual void _OnPhysicsProcess(double delta)
    {
        // Override in subclasses
    }

    public virtual void _OnNextTransitions()
    {
        // Override in subclasses
    }

    public virtual void _OnEnter()
    {
        // Override in subclasses
    }

    public virtual void _OnExit()
    {
        // Override in subclasses
    }
}
