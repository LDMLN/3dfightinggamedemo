using Godot;
using System;

public partial class MenuSwap : Button
{
    [Export]
    Node SwitchToMenu;
    [Export]
    AudioStreamPlayer buttonSFX;

    public override void _Ready()
    {
        Pressed += OnMenuSwapperButtonPressed;
    }

    private void OnMenuSwapperButtonPressed()
    {
        // button sfx
        buttonSFX.Play();
        
        // Need 3 GetParents because the button is 3 nodes down from the MenuTab
        if (GetParent().GetParent().GetParent() is MenuTab menuTab)
        {
            menuTab.OnMenuSwapButtonPressed(SwitchToMenu.GetIndex());
        }
    }
}
