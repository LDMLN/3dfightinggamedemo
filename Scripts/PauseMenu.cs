using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    [Export] bool visibleOnPause = true;

    public override void _Ready()
    {
        PauseManager.Instance.GamePausedToggle += ToggleVisibility;

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
