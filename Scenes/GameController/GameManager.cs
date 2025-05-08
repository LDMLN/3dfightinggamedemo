using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance {get; private set;}

    [Signal]
    public delegate void GamePauseToggleEventHandler(bool isPaused);

    private bool isPaused = false;

    public override void _Ready()
    {
        Instance = this;
    }



    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
        {
            // do nothing if it's not the pause key **NOTE NEED TO SWAP FROM Key.Tab to Input Map key PAUSE
            if(inputEventKey.Keycode != Key.Tab) return;

            // else, update Pause
            GamePause();
        }
    }

    // function to quit game
    public void OnQuitGameBtnPressed()
    {
        GetTree().Quit();
    }

    // function to resume game
    public void OnReturnGameBtnPressed()
    {
        GamePause();
    }

    // pause function
    public void GamePause()
    {
        isPaused = !isPaused;
        EmitSignal(SignalName.GamePauseToggle, isPaused);
        GetTree().Paused = isPaused;
    }
}