using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance {get; private set;}
    [Export]
    AudioStreamPlayer buttonSFX;

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
            isPaused = !isPaused;
            EmitSignal(SignalName.GamePauseToggle, isPaused);
            GetTree().Paused = isPaused;
        }
    }

    // function to quit game
    public void OnQuitGameBtnPressed()
    {
        // button sfx
        buttonSFX.Play();

        // quit functionality
        GetTree().Quit();
    }

    // function to resume game
    public void OnReturnGameBtnPressed()
    {
        // button sfx
        buttonSFX.Play();

        // pause functionality
        isPaused = !isPaused;
        EmitSignal(SignalName.GamePauseToggle, isPaused);
        GetTree().Paused = isPaused;
    }
}