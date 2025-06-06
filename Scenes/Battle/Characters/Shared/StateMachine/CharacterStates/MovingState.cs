using Godot;
using System;
using System.Threading.Tasks;

public partial class MovingState : CharacterState
{
	Vector3 velocity = Vector3.Zero;

	private float dashModifier = 4.0f;
	bool isDashing = false;

	public override void Enter(string movementInput, string attackInput)
	{
		//GD.Print("Enter(Inputs version!): movement: " + movementInput, "\nattack: " + attackInput);
		stateLabel.Text = "Moving";
		stateLabel.Modulate = Color.Color8(255, 0, 255, 255);
		//we are only concered with THIS movement, the button just pressed. nothing previous matters (yet)
		velocity = Vector3.Zero;
		//we are passing the previous input into the handleInput so that it can react to what was pressed that triggered Idle->Move state transition
		HandleMovement(movementInput);
	}

	public override void SpecialEnter(string name)
	{
		//GD.Print("Enter(Inputs version!): movement: " + movementInput, "\nattack: " + attackInput);
		stateLabel.Text = "Moving";
		stateLabel.Modulate = Color.Color8(255, 0, 255, 255);
		//we are only concered with THIS movement, the button just pressed. nothing previous matters (yet)
		velocity = Vector3.Zero;
		//we are passing the previous input into the handleInput so that it can react to what was pressed that triggered Idle->Move state transition
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
		character.Velocity = new Vector3(velocity.X, 0, velocity.Z);
		character.MoveAndSlide();
	}

	public override void HandleInput(string movementInput, string attackInput)
	{
		if (isDashing) { return;  }
		GD.Print("handling input inside of Moving State");
		if (attackInput != "")
		{
			EmitSignal(SignalName.TransitionRequested, (int)State.Moving, (int)State.Attacking, movementInput, attackInput);
		}
		else if (movementInput == "5")
		{
			EmitSignal(SignalName.TransitionRequested, (int)State.Moving, (int)State.Idle, movementInput, attackInput);
		}
		//we don't move if we're attacking
		else if (cardinals.Contains(movementInput))
		{
			HandleMovement(movementInput);
		}
		else
		{
			if (movementInput == "7" || movementInput == "9")
			{
				EmitSignal(SignalName.TransitionRequested, (int)State.Moving, (int)State.Jumping, movementInput, attackInput);
			}
			else if (movementInput == "1" || movementInput == "3")
			{
				EmitSignal(SignalName.TransitionRequested, (int)State.Moving, (int)State.Crouching, movementInput, attackInput);
			}
		}
	}

	public override void ForceSpecialInputTransition(string name, State targetState)
	{
		if (state == targetState)
		{
			GD.Print("Hi, I'm Moving State reading a special move that matches me");
		}
		else
		{
			GD.Print("Hi, I'm Moving state, reading a special move that wants to go: " + targetState);
			EmitSignal(SignalName.SpecialTransitionRequested, (int)State.Moving, (int)targetState, name);
		}
    }

	private void HandleMovement(string movementInput)
	{
		float horizontalSpeed = 0;
		float strafeSpeed = 0;
		Vector2 blendPosition = Vector2.Zero;

		//this is cleaner, imo, but might be a problem when trying to handle things like (if "8" is held, we jump) later so I'm not using it... might use later though.
		//strafeSpeed = movementInput == "8" ? 1 : movementInput == "2" ? -1 : 0;
		//horizontalSpeed = movementInput == "6" ? 1 : movementInput == "4" ? -1 : 0;

		//if held, switch to jumping state...not implemented
		if (movementInput == "8")
		{
			strafeSpeed = character.leftSide ? 1 : -1;
			blendPosition = character.leftSide ? new Vector2(-1, 0) : new Vector2(1, 0); // Left in BlendSpace
		}
		//if held, switch to crouching state... not implemented
		else if (movementInput == "2")
		{
			strafeSpeed = character.leftSide ? -1 : 1;
			blendPosition = character.leftSide ? new Vector2(1, 0) : new Vector2(1, 0); // Right in BlendSpace
		}
		else if (movementInput == "6")
		{
			horizontalSpeed = character.leftSide ? 1 : -1;
			blendPosition = character.leftSide ? new Vector2(0, 1) : new Vector2(0, -1); // Forward in BlendSpace
		}
		else if (movementInput == "4")
		{
			horizontalSpeed = character.leftSide ? -1 : 1;
			blendPosition = character.leftSide ? new Vector2(0, -1) : new Vector2(0, 1); // Backward in BlendSpace
		}
		else
		{
			GD.Print("STOP MOVING!!!!");
			horizontalSpeed = 0;
			strafeSpeed = 0;
			//no longer necessary since we switch to this animation automatically upon entered IdleState
			//blendPosition = Vector2.Zero; // Idle position in BlendSpace
		}

		// Apply the blend position to the animation tree
		if (animTree != null && animTree.Active)
		{
			animTree.Set("parameters/Locomotion/blend_position", blendPosition);
		}

		//we will only be moving either forward/backward or "up"/"down"
		horizontalSpeed = strafeSpeed == 0 ? horizontalSpeed : 0;
		strafeSpeed = horizontalSpeed == 0 ? strafeSpeed : 0;

		Vector3 directionToEnemy = character.GlobalPosition.DirectionTo(enemyCharacter.GlobalPosition);

		if (horizontalSpeed != 0)
		{
			velocity += directionToEnemy * horizontalSpeed * characterMovementSpeed;
		}
		//this has two bugs:
		//1.) the initial movement goes in a straight line and then rounds out to a curve...
		//2.) the curve gets wider if looped many times around the enemy character
		if (strafeSpeed != 0)
		{
			Vector3 strafeTarget = directionToEnemy.Rotated(Vector3.Up, (float)(Math.PI / 2));
			velocity = strafeTarget.Normalized() * characterMovementSpeed * strafeSpeed;
			velocity.Y = 0;
		}
		//check to make sure everything doesn't get out of control.
		velocity = velocity.LimitLength(characterMovementSpeed);
		//this is probably unnecessary now since we default to a 0 speed when entering this state and we cannot enter this state if one of the cardinals was not pressed...
		velocity = (horizontalSpeed == 0 && strafeSpeed == 0) ? Vector3.Zero : velocity;
	}

	public override async void HandleSpecialInput(string specialInputName)
	{
		GD.Print("Moving state has to handle special moves Dash and Backdash");
		isDashing = true;
		float horizontalSpeed = 0;
		float strafeSpeed = 0;
		Vector2 blendPosition = Vector2.Zero;

		//we already reversed everything when getting special inputs so no need to check left side or not
		//only for right and left!!!
		if (specialInputName == "Dash")
		{
			horizontalSpeed = 1;
			GD.Print("character left side? :" + character.leftSide);
			blendPosition = new Vector2(0, 1); // Forward in BlendSpace
		}
		else if (specialInputName == "BackDash")
		{
			horizontalSpeed = -1;
			blendPosition = new Vector2(0, -1); // Backward in BlendSpace
		}
		else if (specialInputName == "UpDash")
		{
			strafeSpeed = character.leftSide ? 1 : -1;
			blendPosition = character.leftSide ? new Vector2(-1, 0) : new Vector2(1, 0); // Left in BlendSpace
		}
		else if (specialInputName == "DownDash")
		{
			strafeSpeed = character.leftSide ? -1 : 1;
			blendPosition = character.leftSide ? new Vector2(1, 0) : new Vector2(1, 0); // Right in BlendSpace
		}
		else
		{
			GD.Print("STOP MOVING!!!!");
			horizontalSpeed = 0;
			strafeSpeed = 0;
			//no longer necessary since we switch to this animation automatically upon entered IdleState
			//blendPosition = Vector2.Zero; // Idle position in BlendSpace
		}

		// Apply the blend position to the animation tree
		if (animTree != null && animTree.Active)
		{
			animTree.Set("parameters/Locomotion/blend_position", blendPosition);
		}

		//we will only be moving either forward/backward or "up"/"down"
		horizontalSpeed = strafeSpeed == 0 ? horizontalSpeed : 0;
		strafeSpeed = horizontalSpeed == 0 ? strafeSpeed : 0;

		Vector3 directionToEnemy = character.GlobalPosition.DirectionTo(enemyCharacter.GlobalPosition);

		if (horizontalSpeed != 0)
		{
			velocity += directionToEnemy * horizontalSpeed * characterMovementSpeed * dashModifier;
		}
		//this has two bugs:
		//1.) the initial movement goes in a straight line and then rounds out to a curve...
		//2.) the curve gets wider if looped many times around the enemy character
		if (strafeSpeed != 0)
		{
			Vector3 strafeTarget = directionToEnemy.Rotated(Vector3.Up, (float)(Math.PI / 2));
			velocity = strafeTarget.Normalized() * characterMovementSpeed * strafeSpeed * dashModifier;
			velocity.Y = 0;
		}
		//check to make sure everything doesn't get out of control.
		GD.Print("special dashing with velocity of: " + velocity);
		velocity = velocity.LimitLength(characterMovementSpeed * dashModifier);
		//this is probably unnecessary now since we default to a 0 speed when entering this state and we cannot enter this state if one of the cardinals was not pressed...
		velocity = (horizontalSpeed == 0 && strafeSpeed == 0) ? Vector3.Zero : velocity;
		//might just have all states bounce back to idle after handle the special input???
		await ToSignal(GetTree().CreateTimer(.1), "timeout");
		isDashing = false;
		EmitSignal(SignalName.SpecialTransitionRequested, (int)State.Moving, (int)State.Idle, specialInputName);
    }


	public override void Exit()
	{
		return;
	}
}
