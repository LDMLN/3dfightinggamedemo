using Godot;
using System;

public partial class AttackingState : CharacterState
{
	// StandardMaterial3D attackRedMat = new StandardMaterial3D() { AlbedoColor = new Color(1.0f, 0.0f, 0.0f) };
	/*
	 * For potential use later:
	 */
	private bool attackInProgress = false;
	private string currentAttack = "";

	public override void Enter(string movementInput, string attackInput)
	{
		stateLabel.Text = "Attacking";
		// stateLabel.Modulate = Color.Color8(255, 0, 0, 255);
		//this is probably unnecessary since MoveAndSlide is not called in this state...
		//but just to be safe
		//we might have some attacks with slight forward/backward movement??
		character.Velocity = Vector3.Zero;
		// characterMesh.SetSurfaceOverrideMaterial(0, attackRedMat);

		/*
		 * For potential use later:
		 */
		attackInProgress = false;

		Attack(movementInput, attackInput);
	}

	public override void SpecialEnter(string name)
	{
		stateLabel.Text = "Attacking";
		// stateLabel.Modulate = Color.Color8(255, 0, 0, 255);
		//this is probably unnecessary since MoveAndSlide is not called in this state...
		//but just to be safe
		//we might have some attacks with slight forward/backward movement??
		character.Velocity = Vector3.Zero;
		// characterMesh.SetSurfaceOverrideMaterial(0, attackRedMat);

		/*
		 * For potential use later:
		 */
		attackInProgress = false;

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
		//GD.Print("handling input from ATTACK STATE:\n " + "movementInput: " + movementInput + "\nattackInput: " + attackInput);
		if (attackInput != "")
		{
			Attack(movementInput, attackInput);
		}
		//this should be dependent on each attack's "recovery" time so it should only go to other states if the move has recovered...
		//maybe we only have the Attack() option and an option to transition to Recovery state here.
		else if (movementInput == "5")
		{
			EmitSignal(SignalName.TransitionRequested, (int)State.Attacking, (int)State.Idle, movementInput, attackInput);
		}
		//we don't move if we're attacking
		else if (cardinals.Contains(movementInput))
		{
			EmitSignal(SignalName.TransitionRequested, (int)State.Attacking, (int)State.Moving, movementInput, attackInput);
		}
		else
		{
			if (movementInput == "7" || movementInput == "9")
			{
				EmitSignal(SignalName.TransitionRequested, (int)State.Attacking, (int)State.Jumping, movementInput, attackInput);
			}
			else if (movementInput == "1" || movementInput == "3")
			{
				EmitSignal(SignalName.TransitionRequested, (int)State.Attacking, (int)State.Crouching, movementInput, attackInput);
			}
		}
	}

	public override void HandleSpecialInput(string specialInputName)
	{
		GD.Print("Attacking state has to handle special moves Fireball");
		Fireball inst = character.fireball.Instantiate<Fireball>();
		Vector3 directionToEnemy = character.GlobalPosition.DirectionTo(enemyCharacter.GlobalPosition);
		character.GetParent().AddChild(inst);
		inst.GlobalPosition = character.GetCharacterCenter().GlobalPosition;
		inst.SetDirectionToEnemy(directionToEnemy);
		inst.LookAt(enemyCharacter.GetCharacterCenter().GlobalPosition);
		EmitSignal(SignalName.SpecialTransitionRequested, (int)State.Attacking, (int)State.Idle, specialInputName);
	}

	public override void ForceSpecialInputTransition(string name, State targetState)
	{
		if (state == targetState)
		{
			GD.Print("Hi, I'm Attacking State reading a special move that matches me");
		}
		else
		{
			GD.Print("Hi, I'm Attacking state, reading a special move that wants to go: " + targetState);
		}
	}

	private void Attack(string movementInput, string attackInput)
	{
		string attackAnimation = "";  // Set animation name based on input

		if (movementInput != "5")
		{
			GD.Print("Command Normal");
		}
		else
		{
			if (attackInput == "P")
			{
				attackAnimation = "Jab Punch";
			}
			else if (attackInput == "HP")
			{
				attackAnimation = "Hook Punch";
			}
			else if (attackInput == "K")
			{
				attackAnimation = "Kick";
			}
			else
			{
				attackAnimation = "Roundhouse Kick";
			}
		}

		if (attackAnimation != "")
		{
			// Trigger attack animation in character's AnimationTree state machine
			PlayAttackAnimation(attackAnimation);
			/*
			 * For potential use later:
			 * - currentAttack: tracks which animation is currently playing.
			 * - attackInProgress: flag that could be used for combo systems.
			 */
			currentAttack = attackAnimation;
			attackInProgress = true;
		}
	}

	//should only exit once we've left the "active" frames of the attack... we need the input buffer functionality now
	public override void Exit()
	{
		characterMesh.SetSurfaceOverrideMaterial(0, baseColorMat);
		attackInProgress = false;
	}
}
