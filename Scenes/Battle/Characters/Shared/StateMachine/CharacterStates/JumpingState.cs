using Godot;
using System;

public partial class JumpingState : CharacterState
{
    public override void Enter(string movementInput, string attackInput)
    {
        stateLabel.Text = "Jumping";
        stateLabel.Modulate = Color.Color8(0, 0, 255, 255);
        character.Velocity = Vector3.Zero;
        Jump(movementInput);
    }

    public override void SpecialEnter(string name)
	{
		stateLabel.Text = "Jumping";
        stateLabel.Modulate = Color.Color8(0, 0, 255, 255);
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
        //have a first check here to make sure the charcter is on the ground... if not, we ignore inputs until back on ground...
        //should have an air attack but this is just a demo, it's fine.
        if (attackInput != "")
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Jumping, (int)State.Attacking, movementInput, attackInput);
        }
        else if (movementInput == "5")
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Jumping, (int)State.Idle, movementInput, attackInput);
        }
        //we don't move if we're attacking
        else if (cardinals.Contains(movementInput))
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Jumping, (int)State.Moving, movementInput, attackInput);
        }
        else if (movementInput == "1" || movementInput == "3")
        {
            EmitSignal(SignalName.TransitionRequested, (int)State.Jumping, (int)State.Crouching, movementInput, attackInput);
        }
        else
        {
            Jump(movementInput);
        }
    }
    
    public override void HandleSpecialInput(string specialInputName)
	{
		GD.Print("Jumping State to handle special move jump???");
		return;
    }

    public override void ForceSpecialInputTransition(string name, State targetState)
    {
        if (state == targetState)
        {
            GD.Print("Hi, I'm Jumping State reading a special move that matches me");
        }
        else
        {
            GD.Print("Hi, I'm Jumping state, reading a special move that wants to go: " + targetState);
        }
    }

    private void Jump(string movementInput)
    {
        if (movementInput == "9")
        {
            GD.Print("SMALL FORWARD HOP");
        }
        else if (movementInput == "7")
        {
            GD.Print("SMALL BACK HOP");
        }
        //also need something if "8" is held
    }


    public override void Exit()
    {
        return;
    }
}
