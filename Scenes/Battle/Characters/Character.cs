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

	public PackedScene fireball = GD.Load<PackedScene>("G:/Godot4Projects/3dfightinggamedemo/Scenes/Battle/SpecialEffects/Fireball.tscn");

	private InputHandler characterInputHandler;
	public enum InputMethod
	{
		Keyboard,
		Joypad
    }
	public InputMethod playerInputMethod = InputMethod.Keyboard;
	

	protected float movementSpeed = 5.0f;
	public string name = "Billy";
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

	/*
	 * Hit Box and Opponent's Hurt Layer:
	 */
	private Area3D _hitBox;
	[Export(PropertyHint.Layers3DPhysics)]
	private uint _enemyHurtLayer;

	/* 
	 * Hurt Box implementation:
	 */
	[Export] private int _maxHealth = 10;
	private int _currentHealth;
	private bool _isDead;
	private Area3D _hurtBox;

	protected AnimationNodeStateMachinePlayback animStateMachine;

	[Signal]
	public delegate void HealthChangedEventHandler(float percentage);
	// [Signal] public delegate void DiedEventHandler();

	string cardinals = "2468";
	[Signal]
	public delegate void CharacterDiedEventHandler();

	public Area3D GetHitBox() => _hitBox;
	public Area3D GetHurtBox() => _hurtBox;


	public override void _Ready()
	{
		// characterMesh = GetNode<MeshInstance3D>("MeshInstance3D");

		// Get references to the new structure components
		armature = GetNode<Node3D>("Armature");
		skeleton = GetNode<Skeleton3D>("Armature/Skeleton3D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animTree = GetNode<AnimationTree>("AnimationTree");
		characterMesh = GetNode<MeshInstance3D>("%Guard02");

		// Activate Animation Tree
		animTree.Active = true;

		// Get state machine playback controller from Animation Tree:
		try
		{
			animStateMachine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");
			if (animStateMachine == null)
			{
				GD.PrintErr("Failed to get AnimationNodeStateMachinePlayback!");
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Exception when getting StateMachine playback: {e.Message}");
		}

		characterCenter = GetNode<Node3D>("%CharacterCenter");

		characterInputHandler = GetNode<InputHandler>("%InputHandler");
		stateMachine = GetNode<CharacterStateMachine>("%CharacterStateMachine");
		characterInputHandler.SetStateMachine(stateMachine);
		characterInputHandler.SetCharacter(this);

		/*
		 * Set Health and get reference to Hurt Box:
		 */
		_currentHealth = _maxHealth;
		_hurtBox = GetNode<Area3D>("Hurt Box");

		/*
		 * Get reference to Hit Box:
		 */
		_hitBox = GetNodeOrNull<Area3D>("Armature/Hit Box");
		if (_hitBox != null)
		{
			// Set Hit Box Collision Mask to be the Opponent's Collision Layer
			_hitBox.CollisionMask = _enemyHurtLayer;

			// Connect signal "_on_hit_box_area_entered"
			_hitBox.AreaEntered += OnHitBoxAreaEntered;
		}
	}

	/// <summary>
	/// Applies damage when hit box enters a hurt box.
	/// </summary>
	/// <param name="hurtBox">The Area3D this hit box entered.</param>
	private void OnHitBoxAreaEntered(Area3D hurtBox)
	{
		if (hurtBox.GetParent() is Character otherCharacter)
		{
			GD.Print("Took damage!");
			otherCharacter.TakeDamage(1); // Damage is hard coded for testing purposes

			// Immediately disable hitbox so it can't hit again
			ActivateHitBox(false);
		}
	}

	/// <summary>
	/// Handles damage and death logic.
	/// </summary>
	/// <param name="amount">The amount of damage to apply.</param>
	public void TakeDamage(int amount)
	{
		GD.Print("Current Health: ", _currentHealth);
		_currentHealth = Mathf.Max(_currentHealth - amount, 0);
		InterruptActions();

		EmitSignal(SignalName.HealthChanged, (float)_currentHealth / _maxHealth);

		if (_currentHealth == 0)
		{
			_isDead = true;
			GD.Print("Fatality!");
			// EmitSignal(SignalName.Died);
			CollisionLayer = 0;
			CollisionMask = 1;

			if (_hurtBox != null)
			{
				_hurtBox.SetDeferred("monitorable", false);
			}

			// emit death signal
			Died();
		}
		else
		{
			if (animPlayer != null && animTree != null && animStateMachine != null)
			{
				// Play hit animation
				animStateMachine.Travel("Hit");
			}
		}
	}

	/// <summary>
	/// Interrupts active actions -- called when a character takes damage.
	/// </summary>
	private void InterruptActions()
	{
		DeactivateAllHitBoxes();
	}

	/// <summary>
	/// Disables active hit boxes to prevent further damage from being registered.
	/// Called when actions are interrupted.
	/// </summary>
	private void DeactivateAllHitBoxes()
	{
		if (_hitBox != null)
		{
			_hitBox.Monitoring = false;
		}
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

	public void ActivateHitBoxTrue() => ActivateHitBox(true);
	public void ActivateHitBoxFalse() => ActivateHitBox(false);

	/// <summary>
	/// Function for activating the Hit Box during attack animations.
	/// </summary>
	/// <param name="active">Boolean parameter for specifying if the Hit Box is activated or not.</param>
	public void ActivateHitBox(bool active)
	{
		if (_hitBox != null)
		{
			_hitBox.Monitoring = active;
		}
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

	public int GetMaxHealth()
	{
		return _maxHealth;
	}

}
