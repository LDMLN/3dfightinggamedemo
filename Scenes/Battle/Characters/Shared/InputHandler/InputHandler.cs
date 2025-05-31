using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class InputHandler : Node
{
	//using this to stop the player2 from reading inputs atm
	[Export] bool active = true;
    protected Character character;

    Dictionary<string, bool> cardinalMovementsHeld = new Dictionary<string, bool> { ["2"] = false, ["4"] = false, ["6"] = false, ["8"] = false };

    ulong upPressedTime;
    ulong upReleasedTime;
    bool upHeld = false;
    bool jumped = false;

    ulong downPressedTime;
    ulong downReleasedTime;
    bool downHeld = false;
    bool crouched = false;

    ulong forwardPressedTime;
    ulong forwardReleasedtime;
    bool forwardHeld = false;

    ulong backPressedTime;
    ulong backReleasedTime;
    bool backHeld = false;

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

    [Signal]
    public delegate void PauseRequestedEventHandler();

    GenericMoveList genericMoveList = new();

    Dictionary<string, Dictionary<string, string>> LeftSideGenericMoveList;
    Dictionary<string, Dictionary<string, string>> RightSideGenericMoveList;
    Dictionary<string, Dictionary<string, string>> currentMoveDictionary;

	private PlayerInput currentInput;
	private CharacterStateMachine characterStateMachine;

	const int DESIRED_FRAME_BUFFER = 12;

	//this is used so we can check if we are moving "back" into neutral...
	//otherwise we will have a constant "input" read of "5" being inserted into the input reader...
	//hmmmmm but this is annoying... we don't have any ewgf movements so maybe we just ignore for simplicity???
	private bool lastMovementInputWasNeutral = false;

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
        LeftSideGenericMoveList = genericMoveList.GetLeftSideGenericMoveList();
        RightSideGenericMoveList = genericMoveList.GetRightSideGenericMoveList();
    }


	public override void _Process(double delta)
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
        if (Input.IsActionPressed("Pause"))
        {
            GD.Print("pause called");
            EmitSignal(SignalName.PauseRequested);
        }

        //need to do the motion check here because when a controller is plugged in, godot will constantly check
        //and receivie JoypadMotion inputs... really messes things up.
        if (!active || @event is InputEventJoypadMotion)
        {
            return;
        }

        if (character.playerInputMethod == Character.InputMethod.Keyboard && @event is not InputEventKey)
        {
            return;
        }

        if (character.playerInputMethod == Character.InputMethod.Joypad && @event is InputEventKey)
        {
            return;
        }

        string foundMove = "";
        
        elapsedTime = Time.GetTicksMsec() - startTime;

        //have to handle this here instead of inside the "translateInput" function because of how
        //the "IsActionJustReleased" call works unfortunately.
        if (Input.IsActionJustPressed("Walk_Up") || Input.IsActionJustReleased("Walk_Up") || Input.IsActionPressed("Walk_Up"))
        {
            if (Input.IsActionJustPressed("Walk_Up"))
            {
                GD.Print("got a press up input!");
                upPressedTime = elapsedTime;
                jumped = false;
                cardinalMovementsHeld["8"] = false;
            }
            else if (Input.IsActionPressed("Walk_Up"))
            {
                GD.Print("up still pressed============");
                if (Math.Abs((decimal)elapsedTime - upPressedTime) > bufferFrames)
                {
                    jumped = true;
                    cardinalMovementsHeld["8"] = true;
                    GD.Print("jump Up");
                }
            }
            else if (Input.IsActionJustReleased("Walk_Up"))
            {
                GD.Print("got a release input!");
                GD.Print("Time now: " + elapsedTime);
                GD.Print("when Up pressed: " + upPressedTime);
                if (!jumped & Math.Abs((decimal)elapsedTime - upPressedTime) < bufferFrames)
                {
                    cardinalMovementsHeld["8"] = false;
                    GD.Print("move up");
                }
            }
        }
        else if (Input.IsActionJustPressed("Walk_Down") || Input.IsActionJustReleased("Walk_Down") || Input.IsActionPressed("Walk_Down"))
        {
            if (Input.IsActionJustPressed("Walk_Down"))
            {
                GD.Print("got a press down input!");
                downPressedTime = elapsedTime;
                jumped = false;
                cardinalMovementsHeld["2"] = false;
            }
            else if (Input.IsActionPressed("Walk_Down"))
            {
                GD.Print("up still pressed============");
                if (Math.Abs((decimal)elapsedTime - downPressedTime) > bufferFrames)
                {
                    jumped = true;
                    cardinalMovementsHeld["2"] = true;
                    GD.Print("Crouch");
                }
            }
            else if (Input.IsActionJustReleased("Walk_Down"))
            {
                GD.Print("got a release input!");
                GD.Print("Time now: " + elapsedTime);
                GD.Print("when Up pressed: " + downPressedTime);
                cardinalMovementsHeld["2"] = false;
                GD.Print("move up");
            }
        }
        else if (Input.IsActionJustPressed("Walk_Left") || Input.IsActionJustReleased("Walk_Left") || Input.IsActionPressed("Walk_Left"))
        {
            if (Input.IsActionJustPressed("Walk_Left"))
            {
                GD.Print("got a press down input!");
                backPressedTime = elapsedTime;
                cardinalMovementsHeld["4"] = false;
            }
            else if (Input.IsActionPressed("Walk_Left"))
            {
                GD.Print("up still pressed============");
                if (Math.Abs((decimal)elapsedTime - backPressedTime) > bufferFrames)
                {
                    cardinalMovementsHeld["4"]  = true;
                    GD.Print("moving back");
                }
            }
            else if (Input.IsActionJustReleased("Walk_Left"))
            {
                GD.Print("got a release input!");
                GD.Print("Time now: " + elapsedTime);
                GD.Print("when Up pressed: " + backPressedTime);
                cardinalMovementsHeld["4"]  = false;
                GD.Print("move up");
            }
        }
        else if (Input.IsActionJustPressed("Walk_Right") || Input.IsActionJustReleased("Walk_Right") || Input.IsActionPressed("Walk_Right"))
        {
            if (Input.IsActionJustPressed("Walk_Right"))
            {
                GD.Print("got a press down input!");
                forwardPressedTime = elapsedTime;
                cardinalMovementsHeld["6"]  = false;
            }
            else if (Input.IsActionPressed("Walk_Right"))
            {
                GD.Print("up still pressed============");
                if (Math.Abs((decimal)elapsedTime - downPressedTime) > bufferFrames)
                {
                    cardinalMovementsHeld["6"] = true;
                    GD.Print("moving forward");
                }
            }
            else if (Input.IsActionJustReleased("Walk_Right"))
            {
                GD.Print("got a release input!");
                GD.Print("Time now: " + elapsedTime);
                GD.Print("when Up pressed: " + forwardPressedTime);
                cardinalMovementsHeld["6"] = false;
            }
        }
        
        currentInput = TranslateInput(@event);
        GD.Print("from " + character.name + " current input movement: " + currentInput.movementInput);
        currentMoveDictionary = character.leftSide ? LeftSideGenericMoveList : RightSideGenericMoveList;
        
        if (@event is InputEventMouseMotion && currentInput.attackInput == "" && currentInput.movementInput == "5")
        {
            return;
        }
        UpdateInputBuffer(currentInput, upHeld, downHeld, forwardHeld, backHeld);
        //quick bit of code to prove that it works
        string recentInputs = "";
        foreach (PlayerInput recentInput in playerInputList)
        {
            if (recentInput.attackInput == "")
            {
                recentInputs += recentInput.movementInput;
            }
            else
            {
                recentInputs += recentInput.attackInput;
            }
            if (currentMoveDictionary.ContainsKey(recentInputs))
            {
                GD.Print(character.Name + "used: " + currentMoveDictionary[recentInputs]["name"]);
                foundMove = recentInputs;
            }
        }
        //GD.Print("recent inputs: " + recentInputs);

        if (foundMove != "")
        {
            GD.Print("caling the special Input pass function from Input Handler! From: " + character.name);
            characterStateMachine.PassSpecialInputToState(currentMoveDictionary[foundMove]);
            playerInputList.Clear();
        }
        else
        {
            GD.Print("calling the not a special input passing function");
            characterStateMachine.PassInputToState(currentInput.movementInput, currentInput.attackInput);
        }
    }

    private void UpdateInputBuffer(PlayerInput currentInput, bool upHeld, bool downHeld, bool forwardHeld, bool backHeld)
    {
        //this could use some more special cases, but as of right now a neutral input does not reset the input buffer
        if (currentInput.movementInput == "5")
        {
            lastMovementInputWasNeutral = true;
        }
        //TODO:
        if (currentInput.movementInput != "5" && currentInput.attackInput != "")
        {
            GD.Print("adding an attack input!" + currentInput.attackInput);
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
            GD.Print("adding an attack input!" + currentInput.attackInput);
            if (playerInputList.Count > 0 && Math.Abs((decimal)playerInputList.Last().inputTime - elapsedTime) < bufferFrames)
            {
                if (cardinalMovementsHeld.ContainsKey(currentInput.movementInput) && cardinalMovementsHeld[currentInput.movementInput])
                {
                    int lastHeldIndex = FindMostRecentXInputInInputList(playerInputList, currentInput.movementInput);
                    playerInputList[lastHeldIndex] = currentInput;
                }
                else
                {
                    playerInputList.Add(currentInput);
                }
            }
            else
            {
                playerInputList.Clear();
                playerInputList.Add(currentInput);
            }
        }
        else if (currentInput.attackInput != "")
        {
            GD.Print("adding an attack input!" + currentInput.attackInput);
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
    }

    private static int FindMostRecentXInputInInputList(List<PlayerInput> playerInputList, string movementInput)
    {
        for (int currentIndex = playerInputList.Count() - 1; currentIndex > -1; currentIndex--)
        {
            if (playerInputList[currentIndex].movementInput == movementInput)
            {
                return currentIndex;
            }
        }
        //-1?
        return 0;
    }


    public PlayerInput TranslateInput(InputEvent @event)
    {
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
        //TODO: build off of this, trying to get tap/vs hold vs release figured out
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

    public void SetCharacter(Character currentCharacter)
    {
        character = currentCharacter;
    }
}
