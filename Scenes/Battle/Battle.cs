using Godot;
using System;

public partial class Battle : Node3D
{
	BattleCamera battleCamera;
	Character player1;
	Character player2;
	Stage battleStage;
	HealthBar player1Health;
	HealthBar player2Health;
	
	[Signal]
	public delegate void BattleReadyEventHandler();

	public override void _Ready()
	{
		battleCamera = GetNode<BattleCamera>("%BattleCamera");
		player1 = GetNode<Character>("Character");
		player2 = GetNode<Character>("EnemyCharacter");
		battleStage = GetNode<Stage>("OceanStage");

		// prep health bars
		player1Health = GetNode<HealthBar>("HUD/Control/PlayerHealthBar");
		player2Health = GetNode<HealthBar>("HUD/Control/EnemyHealthBar");

		player1Health.SetPlayer(player1);
		player1Health.InitHealth(player1.GetMaxHealth());

		player2Health.SetPlayer(player2);
		player2Health.InitHealth(player2.GetMaxHealth());

		//player1.SetBattleCamera(battleCamera);
		player1.InitializeStateMachine(player2, battleCamera);
		player2.InitializeStateMachine(player1, battleCamera);
		//player2.SetBattleCamera(battleCamera);

		EmitSignal(SignalName.BattleReady);

		battleCamera.SetPlayers(player1, player2);
		battleStage.BattleStart();
	}

}
