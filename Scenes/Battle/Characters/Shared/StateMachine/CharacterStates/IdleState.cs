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

	public override void Exit()
	{
		return;
	}


}
