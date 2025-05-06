using Godot;
using System;

public partial class CharacterStateMachine : Node
{
    //currently setting in editor... might just have it set in the _Ready loop later
    [Export] public CharacterState initialState;
    public CharacterState currentState;
    //update this if we end up needing more/less CharacterStates
    public CharacterState[] states = new CharacterState[8];

    //maybe but this in Init and have it be called from Character so we can guarantee
    //character -> statemachine order of initialization
    public void Init(Character character, Character targetCharacter, BattleCamera battleCamera)
    {
        foreach (CharacterState childState in GetChildren())
        {
            //since enums are just ints with names, basically we are saying that
            //states[(int)childState.state] which means states[1] = childState
            states[(int)childState.state] = childState;
            //when a state wants to transition, this hooks it up to the below method
            childState.TransitionRequested += OnTransitionRequested;
            //sets the character the state is handling
            childState.SetCharacter(character);
            //sets the target character and camera, necessary for 3d/lock-on movement
            childState.SetTargetAndCamera(targetCharacter, battleCamera);
            childState.SetAnimationNodes();
            childState.SetDebugNodes();
        }

        if (initialState != null)
        {
            //this is the initial Enter so no movement/attack inputs... we could fake this and
            //cut out the base implementation of Enter by passing "5" "" ??? hmmmm
            initialState.Enter();
            currentState = initialState;
        }
    }

    //we are going to call our self-made Process function inside each state every frame
    public override void _Process(double delta)
    {
        currentState?.Update(delta);
        //GD.Print(currentState.state);
    }

    //same as above but with physics tick
    public override void _PhysicsProcess(double delta)
    {
        currentState?.PhysicsUpdate(delta);
    }

    //same as above but with inputs... we do this right???
    //maybe we should call the translate input function in this stateMachine??
    //or call it in the IdleState and from here just pass @event????
    public void PassInputToState(string movementInput, string attackInput)
    {
        currentState?.HandleInput(movementInput, attackInput);
        //GD.Print(newInputs);
    }




    private void OnTransitionRequested(int From, int To, string MovementInput, string AttackInput)
    {
        GD.Print("transition requestion... From: " + From + " To: " + To);
        if (states[From] != currentState) { return; }
        CharacterState nextState = states[To];
        if (nextState == null) { return; }

        //exit the current state if we have it
        currentState?.Exit();
        //we are adding this because we want the states to be responsive to immediate inputs,
        //not only inputs that are held/with a delay... so we have to pass the previous inputs to it
        nextState.Enter(MovementInput, AttackInput);
        currentState = nextState;
    }
}
