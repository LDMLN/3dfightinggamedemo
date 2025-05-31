using Godot;
using System;

public partial class IdleState : CharacterState
{
	//this is the initial "enter" when state machine initialized... we could get rid of this by passed in a "5" "" in the statemachine script initially...
	public override void Enter()
	{
		stateLabel.Text = "Idle";
		stateLabel.Modulate = Color.Color8(255, 255, 255, 255);
		if (animTree != null && animTree.Active)
		{
			//why is this how we set the idle animation? with a vector2??
			animTree.Set("parameters/Locomotion/blend_position", Vector2.Zero);
		}
		//this is probably unnecessary since MoveAndSlide is not called in this state...
		//but just to be safe
		character.Velocity = Vector3.Zero;
	}

	public override void SpecialEnter(string name)
	{
		GD.Print("special entered into Idle");
		stateLabel.Text = "Idle";
		stateLabel.Modulate = Color.Color8(255, 255, 255, 255);
		if (animTree != null && animTree.Active)
		{
			//why is this how we set the idle animation? with a vector2??
			animTree.Set("parameters/Locomotion/blend_position", Vector2.Zero);
		}
		//this is probably unnecessary since MoveAndSlide is not called in this state...
		//but just to be safe
		character.Velocity = Vector3.Zero;
		HandleSpecialInput(name);
    }


	public override void Enter(string movementInput, string attackInput)
	{
		stateLabel.Text = "Idle";
		stateLabel.Modulate = Color.Color8(255, 255, 255, 255);
		if (animTree != null && animTree.Active)
		{
			//why is this how we set the idle animation? with a vector2??
			animTree.Set("parameters/Locomotion/blend_position", Vector2.Zero);
		}
		//this is probably unnecessary since MoveAndSlide is not called in this state...
		//but just to be safe
		character.Velocity = Vector3.Zero;
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
			EmitSignal(SignalName.TransitionRequested, (int)State.Idle, (int)State.Attacking, movementInput, attackInput);
		}
		//we don't move if we're attacking
		else if (cardinals.Contains(movementInput))
		{
			EmitSignal(SignalName.TransitionRequested, (int)State.Idle, (int)State.Moving, movementInput, attackInput);
		}
		else
		{
			if (movementInput == "7" || movementInput == "9")
			{
				EmitSignal(SignalName.TransitionRequested, (int)State.Idle, (int)State.Jumping, movementInput, attackInput);
			}
			else if (movementInput == "1" || movementInput == "3")
			{
				EmitSignal(SignalName.TransitionRequested, (int)State.Idle, (int)State.Crouching, movementInput, attackInput);
			}
		}
	}

	public override void HandleSpecialInput(string specialInputName)
	{
		GD.Print("Idle state has no special inputs currently");
		return;
    }


	public override void ForceSpecialInputTransition(string name, State targetState)
	{
		if (state == targetState)
		{
			GD.Print("Hi, I'm Idle State reading a special move that matches me");
			HandleSpecialInput(name);
		}
		else
		{
			GD.Print("Hi, I'm Idle state, reading a special move that wants to go: " + targetState);
			EmitSignal(SignalName.SpecialTransitionRequested, (int)State.Idle, (int)targetState, name);
		}
	}


	public override void Exit()
	{
		return;
	}


}
