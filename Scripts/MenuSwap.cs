using Godot;
using System;

public partial class MenuSwap : Button
{
    [Export] Node SwitchToMenu;

    public override void _Ready()
    {
        Pressed += OnMenuSwapperButtonPressed;
    }

    private void OnMenuSwapperButtonPressed()
    {   
        // Need 3 GetParents because the button is 3 nodes down from the MenuTab
        if(GetParent().GetParent().GetParent() is MenuTab menuTab){
            menuTab.OnMenuSwapButtonPressed(SwitchToMenu.GetIndex());
        }
    }
}
