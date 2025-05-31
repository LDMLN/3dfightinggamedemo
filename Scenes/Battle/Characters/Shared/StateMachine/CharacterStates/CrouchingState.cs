using Godot;
using System;

public partial class CrouchingState : CharacterState
{
    public override void Enter(string movementInput, string attackInput)
    {
        stateLabel.Text = "Crouching";
        stateLabel.Modulate = Color.Color8(0, 255, 0, 255);
        character.Velocity = Vector3.Zero;
        Crouch(movementInput);
    }

    public override void SpecialEnter(string name)
	{
		stateLabel.Text = "Crouching";
        stateLabel.Modulate = Color.Color8(0, 255, 0, 255);
        character.Velocity = Vector3.Zero;
        HandleSpecialInput(name);
	}

    //right now it's not doing anything that all other states aren't doing (checking left side)
    public override void Update(double delta)
    {
        base.Update(delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);
    }

    public override void HandleInput(string movementInput, string attackInput)
    {
        if (attackInput != "")
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Crouching, (int)State.Attacking, movementInput, attackInput);
        }
        else if (movementInput == "5")
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Crouching, (int)State.Idle, movementInput, attackInput);
        }
        else if (cardinals.Contains(movementInput))
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Crouching, (int)State.Moving, movementInput, attackInput);
        }
        else if (movementInput == "7" || movementInput == "9")
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Crouching, (int)State.Jumping, movementInput, attackInput);
        }
        else
        {
            Crouch(movementInput);
        }
    }
    
    public override void HandleSpecialInput(string specialInputName)
	{
		GD.Print("Crouching State to handle special move crouch???");
		return;
    }

    public override void ForceSpecialInputTransition(string name, State targetState)
    {
        if (state == targetState)
        {
            GD.Print("Hi, I'm Crouching State reading a special move that matches me");
        }
        else
        {
            GD.Print("Hi, I'm Crouching state, reading a special move that wants to go: " + targetState);
        }
    }

    private void Crouch(string movementInput)
    {
        if (movementInput == "1")
        {
            GD.Print("CROUCH");
        }
        else if (movementInput == "3")
        {
            GD.Print("CROUCH AND MOVE FORWARD");
        }
        //we also need something for if "2" held down for a simple crouch
    }


    public override void Exit()
    {
        return;
    }
}
