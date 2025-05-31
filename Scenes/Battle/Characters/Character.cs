using Godot;
using System;

//author: Luiz Leao
[GlobalClass]
public partial class Character : CharacterBody3D
{
	//this has to be passed into it from the the Battle scene
	[Export] public Character enemyCharacter;
	//this is unneccessary later... right now just only works if the int is "1" 
	[Export(PropertyHint.Range, "1,2")] public int whichPlayer;

	private InputHandler characterInputHandler;

	float movementSpeed = 5.0f;
	Vector3 velocity = Vector3.Zero;
	Vector3 targetPosition = Vector3.Zero;

	// Reference to the Imported Model's Armature and Skeleton3D
	private Node3D armature;
	private Skeleton3D skeleton;
	private MeshInstance3D characterMesh;

	// Animation player and tree for controlling animations
	private AnimationPlayer animPlayer;
	private AnimationTree animTree;

	private BattleCamera battleCamera;

	private CharacterStateMachine stateMachine;

	public bool leftSide;

	private Node3D characterCenter;

	[Signal]
	public delegate void CharacterDiedEventHandler();

	public override void _Ready()
	{
		// characterMesh = GetNode<MeshInstance3D>("MeshInstance3D");
		// baseColorMat = (StandardMaterial3D)characterMesh.GetSurfaceOverrideMaterial(0);

		// Get references to the new structure components
		armature = GetNode<Node3D>("Armature");
		skeleton = GetNode<Skeleton3D>("Armature/Skeleton3D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animTree = GetNode<AnimationTree>("AnimationTree");
		characterMesh = GetNode<MeshInstance3D>("%Guard02");
		characterCenter = GetNode<Node3D>("%CharacterCenter");

		characterInputHandler = GetNode<InputHandler>("%InputHandler");
		stateMachine = GetNode<CharacterStateMachine>("%CharacterStateMachine");
		characterInputHandler.SetStateMachine(stateMachine);

	}

	public void InitializeStateMachine(Character targetCharacter, BattleCamera currentBattleCamera)
	{
		stateMachine.Init(this, targetCharacter, currentBattleCamera);
	}

	public override void _Process(double delta)
	{
		return;
	}

	public override void _PhysicsProcess(double delta)
	{
		return;
	}

	public void SetEnemyCharacter(Character enemy)
	{
		enemyCharacter = enemy;
	}

	private void Died()
	{
		EmitSignal(SignalName.CharacterDied);
	}

	public Node3D GetCharacterCenter()
	{
		return characterCenter;
	}

	public void SetBattleCamera(BattleCamera camera)
	{
		battleCamera = camera;
	}

	public float GetMovementSpeed()
	{
		return movementSpeed;
	}

	public MeshInstance3D GetCharacterMesh()
	{
		return characterMesh;
	}

}
