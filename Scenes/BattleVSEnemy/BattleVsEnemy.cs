using Godot;
using System;

public partial class BattleVsEnemy : Battle
{
    BattleCamera battleCamera;
    Character player1;
    Character player2;
    Stage battleStage;
    
    public override void _Ready()
	{
		battleCamera = GetNode<BattleCamera>("%BattleCamera");
		player1 = GetNode<Character>("Character");
		player2 = GetNode<Character>("Enemy");
		battleStage = GetNode<Stage>("OceanStage");

		player1.whichPlayer = 1;
		player1.InitializeStateMachine(player2, battleCamera);
		player2.whichPlayer = 2;
		
        // set up the Enemy
        var enemy = GetNode<CharacterBody3D>("Enemy");
        var player = GetNode<CharacterBody3D>("Character");

        var stateMachine = enemy.GetNode<Node>("NodeStateMachine");
        var navAgent = enemy.GetNode<NavigationAgent3D>("NavigationAgent3D");

        foreach (var child in stateMachine.GetChildren())
        {
            if (child is NodeState state)
            {
                state.SetTarget(player);
                state.SetAgent(navAgent);
            }
        }

		battleCamera.SetPlayers(player1, player2);
		battleStage.BattleStart();
	}
}
