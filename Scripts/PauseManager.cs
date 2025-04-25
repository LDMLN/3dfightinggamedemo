using Godot;
using System;

public partial class PauseManager : Node
{
    public static PauseManager Instance {get; private set; }

    [Signal]
    public delegate void GamePausedToggleEventHandler(bool isPaused);

    private bool isPaused = false;

    // called when node enters the scene tree for the first time
    public override void _Ready()
    {
        Instance = this;
    }

    // callback method that runs every time an input event happens (key presses, mouse movement, etc)
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
        {
            if(inputEventKey.Keycode != Key.Escape) return;

            isPaused = !isPaused;

            EmitSignal(SignalName.GamePausedToggle, isPaused);
            GetTree().Paused = isPaused;
        }
    }
}
