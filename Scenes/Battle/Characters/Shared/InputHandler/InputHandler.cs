using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class InputHandler : Node
{
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

    private int frameCounter = 0;

    List<string> inputList = new List<string>();

    public override void _Process(double delta)
    {
        return;
    }
    
    //TODO: have to figure out a way to have this reset with 6 frames between each press
    //IF the next press leads us to possible having a move triggered... this is how things are handled in sf, but seems complicated...
    //I'll try and figure it out!
    public override void _PhysicsProcess(double delta)
    {
        frameCounter++;
        if(frameCounter % 6 == 0)
        {
            //GD.Print("input array cleared!");
            frameCounter = 0;
            GD.Print(string.Join(",", inputList));
            GD.Print("=================CLEARING INPUT ARRAY=============================");
            inputList.Clear();
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Pause Menu Check
        
        
        //the character class is translating the input and then passing to the state machine...
        //feels gross... should probalby have an "inputHandler" class that does this...
        currentInput = TranslateInput(@event);
        //TODO: also have to fix it so that things are only added to it once pressed or if held down and another button is pressed...
        //except for neutral which should be added when NOTHING is pressed (this is working as is right now)
        inputList.Add(currentInput.movementInput);
        characterStateMachine.PassInputToState(currentInput.movementInput, currentInput.attackInput);
    }


    public PlayerInput TranslateInput(InputEvent @event)
    {
        PlayerInput newInput = new PlayerInput(GetMovementInput(@event), GetAttackInput(@event));
        //we are building the movement string.
        return newInput;
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
