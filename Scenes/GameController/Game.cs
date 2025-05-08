using Godot;
using System;

public partial class Game : Node3D
{
    private MainMenu mainMenu;

    // function to load a scene (function to load in game)
    public void SceneChange(PackedScene loadScene)
    {
        GetTree().Root.AddChild(loadScene.Instantiate());
        QueueFree();
    }


}
