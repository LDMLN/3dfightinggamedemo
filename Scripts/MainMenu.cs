using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MainMenu : Control
{
    //Holds indexes of menus
    List<int> goBacklist = new();

    // function to go to other menus
    public void SwapMenu(int menuIndex, int returnIndex){
        if(GetChild(menuIndex) is MenuTab menuTab){
            menuTab.Visible = true;
        }

        if(returnIndex < 0) return;
        goBacklist.Add(returnIndex);
    }

    // function go back to previous menu
    public void SwapMenuToPrevious(){
        if(!goBacklist.Any()) return;
        SwapMenu(goBacklist[goBacklist.Count - 1], -1);
        goBacklist.RemoveAt(goBacklist.Count - 1);
    }

    // function to load a scene (function to load in game)
    public void OnSwapScene(PackedScene loadScene){
        GetTree().Root.AddChild(loadScene.Instantiate());
        QueueFree();
    }

    // function to quit game
    public void OnQuitGameBtnPressed(){
        GetTree().Quit();
    }
}
