using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class InputHandler : Node
{
	//using this to stop the player2 from reading inputs atm
	[Export] bool active = true;

	public struct PlayerInput
	{
		public ulong inputTime;
		public string movementInput, attackInput;

		public PlayerInput(string movementInput, string attackInput, ulong inputTime)
		{
			this.inputTime = inputTime;
			this.movementInput = movementInput;
			this.attackInput = attackInput;
		}
	}

	Dictionary<string, string> moveList = new Dictionary<string, string> { ["236"] = "FIREBALL", ["63236"] = "DRAGON-UPPER" };

	private PlayerInput currentInput;
	private CharacterStateMachine characterStateMachine;

	const int DESIRED_FRAME_BUFFER = 8;

	//this is used so we can check if we are moving "back" into neutral...
	//otherwise we will have a constant "input" read of "5" being inserted into the input reader...
	//hmmmmm but this is annoying... we don't have any ewgf movements so maybe we just ignore for simplicity???
	private bool lastMovementInputWasNeutral = false;

	private int framesSinceLastInput = 0;
	private int testFrames = 0;

	private ulong startTime = 0;
	private ulong elapsedTime = 0;
	//msec/fps * desiredFrames... is what we want to do, but this is int division, so we're going to force a round up, make it more lenient
	private int bufferFrames = (1000 / Engine.PhysicsTicksPerSecond + 1) * DESIRED_FRAME_BUFFER;

	List<PlayerInput> playerInputList = new List<PlayerInput>();

	public override void _Ready()
	{
		GD.Print("physics ticks: " + Engine.PhysicsTicksPerSecond);
		GD.Print("current frame buffer: " + bufferFrames);
		startTime = Time.GetTicksMsec();
	}


	public override void _Process(double delta)
	{
		return;
	}

	//TODO: have to figure out a way to have this reset with 6 frames between each press
	//IF the next press leads us to possible having a move triggered... this is how things are handled in sf, but seems complicated...
	//I'll try and figure it out!
	public override void _PhysicsProcess(double delta)
	{
		return;
	}

	//TODO: this is working better!
	//we need something that checks whether a button pressed has been released... and until that is true, it CANNOT be added to inputDict again.
	//this will help prevent overrides where we 6HP, but the 6 was SLIGHTLY before the HP and so therefore we get something like
	// 6
	// 6HP
	// when it should just be 
	// 6
	// HP
	public override void _Input(InputEvent @event)
	{
		if (!active)
		{
			return;
		}

		currentInput = TranslateInput(@event);
		if (@event is InputEventMouseMotion && currentInput.attackInput == "" && currentInput.movementInput == "5")
		{
			return;
		}
		UpdateInputBuffer(currentInput);
		// quick bit of code to prove that it works
		if (currentInput.attackInput == "P" || currentInput.attackInput == "HP")
		{
			string recentInputs = "";
			foreach (PlayerInput recentInput in playerInputList)
			{
				recentInputs += recentInput.movementInput;
				if (moveList.ContainsKey(recentInputs))
				{
					GD.Print(moveList[recentInputs]);
				}
			}
		}
		characterStateMachine.PassInputToState(currentInput.movementInput, currentInput.attackInput);
	}

	private void UpdateInputBuffer(PlayerInput currentInput)
	{
		//this could use some more special cases, but as of right now a neutral input does not reset the input buffer
		if (currentInput.movementInput == "5")
		{
			lastMovementInputWasNeutral = true;
		}
		//TODO: also have to fix it so that things are only added to it once pressed or if held down and another button is pressed...
		if (currentInput.movementInput != "5" && currentInput.attackInput != "")
		{
			if (playerInputList.Count > 0 && Math.Abs((decimal)playerInputList.Last().inputTime - elapsedTime) < bufferFrames)
			{
				playerInputList.Add(currentInput);
			}
			else
			{
				playerInputList.Clear();
				playerInputList.Add(currentInput);
			}
		}
		//TODO: we do need someway to handle putting in neutral inputs... even without ewgf movements, it could be useful...
		else if (currentInput.movementInput != "5")
		{
			if (playerInputList.Count > 0 && Math.Abs((decimal)playerInputList.Last().inputTime - elapsedTime) < bufferFrames)
			{
				playerInputList.Add(currentInput);
			}
			else
			{
				playerInputList.Clear();
				playerInputList.Add(currentInput);
			}
		}
		else if (currentInput.attackInput != "")
		{
			if (playerInputList.Count > 0 && Math.Abs((decimal)playerInputList.Last().inputTime - elapsedTime) < bufferFrames)
			{
				playerInputList.Add(currentInput);
			}
			else
			{
				playerInputList.Clear();
				playerInputList.Add(currentInput);
			}
		}

		GD.Print("=================PRINTING CURRENT ARRAY!!!!========================");
		foreach (PlayerInput input in playerInputList)
		{
			GD.Print("[" + input.inputTime + ": " + input.movementInput + " + " + input.attackInput + "],");
		}
	}

	public PlayerInput TranslateInput(InputEvent @event)
	{
		elapsedTime = Time.GetTicksMsec() - startTime;
		return new PlayerInput(GetMovementInput(@event), GetAttackInput(@event), elapsedTime);
		//we are building the movement string.
	}


	private string GetMovementInput(InputEvent @event)
	{
		string movementInput = "";
		//gets whether we are moving up, down, left or right or if opp directions pushed simultaneously
		float depthAxis = Input.GetAxis("Walk_Down", "Walk_Up");
		float horizontalAxis = Input.GetAxis("Walk_Left", "Walk_Right");

		bool up = depthAxis > 0;
		bool down = depthAxis < 0;
		bool right = horizontalAxis > 0;
		bool left = horizontalAxis < 0;

		if (up && right)
		{
			movementInput = "9";
		}
		else if (up && left)
		{
			movementInput = "7";
		}
		else if (down && right)
		{
			movementInput = "3";
		}
		else if (down && left)
		{
			movementInput = "1";
		}
		else if (up)
		{
			movementInput = "8";
		}
		else if (down)
		{
			movementInput = "2";
		}
		else if (right)
		{
			movementInput = "6";
		}
		else if (left)
		{
			movementInput = "4";
		}
		movementInput = movementInput == "" ? "5" : movementInput;

		return movementInput;
	}

	private string GetAttackInput(InputEvent @event)
	{
		string attackInput = "";
		if (Input.IsActionJustPressed("Punch"))
		{
			attackInput += "P";
		}
		if (Input.IsActionJustPressed("Heavy_Punch"))
		{
			attackInput += "HP";
		}
		if (Input.IsActionJustPressed("Kick"))
		{
			attackInput += "K";
		}
		if (Input.IsActionJustPressed("Heavy_Kick"))
		{
			attackInput += "HK";
		}

		return attackInput;
	}

	internal void SetStateMachine(CharacterStateMachine stateMachine)
	{
		characterStateMachine = stateMachine;
	}

}
