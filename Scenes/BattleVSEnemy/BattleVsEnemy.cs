using Godot;
using System;

public partial class BattleVsEnemy : Battle
{
    BattleCamera battleCamera;
    Character player1;
    Character player2;
    Stage battleStage;
    HealthBar playerHealth;
	HealthBar enemyHealth;

    [Signal]
	public delegate void BattleReadyEventHandler();
    
    public override void _Ready()
    {
        battleCamera = GetNode<BattleCamera>("%BattleCamera");
        player1 = GetNode<Character>("Character");
        player2 = GetNode<Character>("Enemy");
        battleStage = GetNode<Stage>("OceanStage");

        // prep health bars
        playerHealth = GetNode<HealthBar>("HUD/Control/PlayerHealthBar");
        enemyHealth = GetNode<HealthBar>("HUD/Control/EnemyHealthBar");

        playerHealth.SetPlayer(player1);
        playerHealth.InitHealth(player1.GetMaxHealth());

        enemyHealth.SetPlayer(player2);
        enemyHealth.InitHealth(player2.GetMaxHealth());

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

        EmitSignal(SignalName.BattleReady);

        battleCamera.SetPlayers(player1, player2);
        battleStage.BattleStart();
    }
}
