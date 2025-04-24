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


        player1.whichPlayer = 1;
        player2.whichPlayer = 2;

        battleCamera.SetPlayers(player1, player2);
        battleStage.BattleStart();
    }

}
