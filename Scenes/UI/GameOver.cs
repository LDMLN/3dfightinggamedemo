using Godot;
using System;

public partial class GameOver : CanvasLayer
{
    [Signal]
    public delegate void PlayAgainEventHandler();

    public void OnPlayAgainBtnPressed()
    {
        EmitSignal(SignalName.PlayAgain);
    }
    
    public void OnQuitGameBtnPressed()
    {
        // quit functionality
        GetTree().Quit();
    }
}
