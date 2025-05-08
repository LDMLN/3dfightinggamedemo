using Godot;
using System;

// Script can be added to any canvas layer node that should be visible on Pause
public partial class ToggleOnPause : CanvasLayer
{
    // variable to set in Godot if canvas layer node should be visible when Paused
    [Export] bool visibleOnPause = true;

    public override void _Ready()
    {
        GameManager.Instance.GamePauseToggle += ToggleVisibility;

        if(!visibleOnPause) return;

        Hide();
    }

    private void ToggleVisibility(bool isPaused)
    {
        if(visibleOnPause == isPaused)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
}
