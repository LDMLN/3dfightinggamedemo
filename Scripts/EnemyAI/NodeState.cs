using Godot;
using System;

public partial class NodeState : Node
{
    [Signal]
    public delegate void TransitionEventHandler(string nextState);

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
