using Godot;
using System;

public partial class LoadScene : Button
{
    [Export]
    PackedScene sceneToSwitchTo;

    [Export]
    AudioStreamPlayer buttonSFX;

    public override void _Ready()
    {
        Pressed += OnSwitchSceneButtonPressed;
    }

    private void OnSwitchSceneButtonPressed()
    {
        // button sfx
        buttonSFX.Play();
        
        // Need 3 GetParents because the button is 3 nodes down from the MenuTab
        if (GetParent().GetParent().GetParent() is MenuTab menuTab)
        {
            menuTab.LoadSceneRequest(sceneToSwitchTo);
        }
    }
}
