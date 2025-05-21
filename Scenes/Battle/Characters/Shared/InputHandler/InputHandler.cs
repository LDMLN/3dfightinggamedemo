using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class InputHandler : Node
{
    [Export] bool active = true;

    public struct PlayerInput
    {
        public string movementInput, attackInput;

		public PlayerInput(string movementInput, string attackInput)
		{
			this.movementInput = movementInput;
			this.attackInput = attackInput;
		}
	}

	private PlayerInput currentInput;
	private CharacterStateMachine characterStateMachine;

    //this is used so we can check if we are moving "back" into neutral...
    //otherwise we will have a constant "input" read of "5" being inserted into the input reader...
    //hmmmmm but this is annoying... we don't have any ewgf movements so maybe we just ignore for simplicity???
    private bool lastMovementInputWasNeutral = false;

    private int framesSinceLastInput = 0;
    private int testFrames = 0;

    private ulong startTime = 0;
    private ulong elapsedTime = 0;
    private int bufferFrames = 102; //should be roughly 6 frames (1000/60*6)

    List<string> inputList = new List<string>();
    Dictionary<ulong, string> inputDict= new Dictionary<ulong, string>();

    public override void _Ready()
    {
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

    //TODO: this still isn't really working... have to fix it.
    public override void _Input(InputEvent @event)
    {
        if (!active)
        {
            return;
        }
        
        currentInput = new PlayerInput();
        currentInput = TranslateInput(@event);
        if (@event is InputEventMouseMotion && currentInput.attackInput == "" && currentInput.movementInput == "5")
        {
            return;
        }
        //the character class is translating the input and then passing to the state machine...
        //feels gross... should probalby have an "inputHandler" class that does this...
        elapsedTime = Time.GetTicksMsec() - startTime;
        if (@event.IsActionPressed("Punch"))
        {
            GD.Print("current input attack button(PRESSED):" + currentInput.attackInput);
        }
        if (@event.IsActionReleased("Punch"))
        {
            GD.Print("current input attack button (RELEASED):" + currentInput.attackInput);
        }
        if (currentInput.attackInput != "" || currentInput.movementInput != "5")
        {
            //GD.Print("!!!!!frame buffer extended!!!!!!");
            framesSinceLastInput = 0;
        }
        //this could use some more special cases, but as of right now a neutral input does not reset the input buffer
        else if (currentInput.movementInput == "5")
        {
            lastMovementInputWasNeutral = true;
        }
        if (inputDict.Count > 0)
        {
            GD.Print("last input's elapsed time: " + inputDict.Last().Key);
            GD.Print("current elapsed time: " + elapsedTime);
            GD.Print("inside the input buffer?" + (Math.Abs((decimal)inputDict.Last().Key - elapsedTime) < bufferFrames));
        }
        //TODO: also have to fix it so that things are only added to it once pressed or if held down and another button is pressed...
        //except for neutral which should be added when NOTHING is pressed (this is working as is right now)
        if (currentInput.movementInput != "5" && currentInput.attackInput != "")
        {
            if (inputDict.Count > 0 && Math.Abs((decimal)inputDict.Last().Key - elapsedTime) < bufferFrames)
            {
                //GD.Print("adding a command normal input! to inputDict and continuing");
                inputDict.Add(elapsedTime,currentInput.movementInput + currentInput.attackInput);
            }
            else
            {
                //GD.Print("clearing inputDict and adding newest input!");
                //inputDict.Clear();
                inputDict.Add(elapsedTime,currentInput.movementInput + currentInput.attackInput);
            }
        }
        else if (!lastMovementInputWasNeutral || currentInput.movementInput != "5")
        {
            if (inputDict.Count > 0 && Math.Abs((decimal)inputDict.Last().Key - elapsedTime) < bufferFrames)
            {
                inputDict.Add(elapsedTime,currentInput.movementInput);
                //GD.Print("adding movement input to input dict!");
            }
            else
            {
                //GD.Print("clearing inputDict and adding newest input!");
                //inputDict.Clear();
                inputDict.Add(elapsedTime,currentInput.movementInput);
            }
            //inputList.Add(currentInput.movementInput);
            //GD.Print(string.Join(",", inputList));
        }
        else if (currentInput.attackInput != "")
        {
            if (inputDict.Count > 0 && Math.Abs((decimal)inputDict.Last().Key - elapsedTime) < bufferFrames)
            {
                inputDict.Add(elapsedTime,currentInput.attackInput);
                //GD.Print("adding movement input to input dict!");
            }
            else
            {
                //GD.Print("clearing inputDict and adding newest input!");
                //inputDict.Clear();
                inputDict.Add(elapsedTime,currentInput.attackInput);
            }
        }
        foreach (ulong key in inputDict.Keys)
        {
            GD.Print("[" + key + ": " + inputDict[key] + "],");
        }
        characterStateMachine.PassInputToState(currentInput.movementInput, currentInput.attackInput);
    }


    public PlayerInput TranslateInput(InputEvent @event)
    {
        return new PlayerInput(GetMovementInput(@event), GetAttackInput(@event));
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
