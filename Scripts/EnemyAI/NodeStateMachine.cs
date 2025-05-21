using Godot;
using System;
using System.Collections.Generic;

public partial class NodeStateMachine : Node
{
    [Export]
    public NodeState InitialNodeState;

    private Dictionary<string, NodeState> _nodeStates = new();
    private NodeState _currentNodeState;
    private string _currentNodeStateName;

    public override void _Ready()
    {
        // sets up and allows children of the nodeState to transition to each other
        foreach (Node child in GetChildren())
        {
            if (child is NodeState nodeState)
            {
                string stateName = child.Name.ToString().ToLower();
                _nodeStates[stateName] = nodeState;
                nodeState.Connect(nameof(NodeState.Transition), new Callable(this, nameof(TransitionTo)));
            }
        }

        // enter initial node
        if (InitialNodeState != null)
        {
            InitialNodeState._OnEnter();
            _currentNodeState = InitialNodeState;
            _currentNodeStateName = InitialNodeState.Name.ToString().ToLower();
        }
    }

    // get current node and enter its OnProcess
    public override void _Process(double delta)
    {
        _currentNodeState?._OnProcess(delta);
        // (C# learning note: ?. is called the null-conditional operator. 
        // -used to safely call a method or access a member only if the object isn’t null.
    }

    // run current node's OnPhysicsProcess and run it's transitions
    public override void _PhysicsProcess(double delta)
    {
        _currentNodeState?._OnPhysicsProcess(delta);
        _currentNodeState?._OnNextTransitions();
    }

    // run current node's transition logic
    private void TransitionTo(string nodeStateName)
    {
        string loweredName = nodeStateName.ToLower();
        if (loweredName == _currentNodeStateName)
            return;

        if (!_nodeStates.TryGetValue(loweredName, out NodeState newState))
            return;

        _currentNodeState?._OnExit();

        newState._OnEnter();
        _currentNodeState = newState;
        _currentNodeStateName = newState.Name.ToString().ToLower();
        GD.Print("Current State: ", _currentNodeStateName);
    }
}
