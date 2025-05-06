using Godot;
using System;

public partial class Battle : Node3D
{
    BattleCamera battleCamera;
    Character player1;
    Character player2;
    Stage battleStage;

    public override void _Ready()
    {
        battleCamera = GetNode<BattleCamera>("%BattleCamera");
        player1 = GetNode<Character>("Character");
        player2 = GetNode<Character>("Character2");
        battleStage = GetNode<Stage>("OceanStage");

        //delete this eventually
        player1.whichPlayer = 1;
        //player1.SetBattleCamera(battleCamera);
        player1.InitializeStateMachine(player2, battleCamera);
        player2.whichPlayer = 2;
        //player2.SetBattleCamera(battleCamera);
        
        //uncomment to turn on player2 movement
        //player2.InitializeStateMachine(player1, battleCamera);

        battleCamera.SetPlayers(player1, player2);
        battleStage.BattleStart();
    }

}
