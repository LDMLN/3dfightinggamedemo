using Godot;
using System;

[GlobalClass]
public abstract partial class CharacterState : Node
{
    public enum State
    {
        Idle,
        Moving,
        Attacking,
        Blocking,
        Jumping,
        Recovery,
        HitStun,
        Crouching
    }

    [Signal]
    public delegate void TransitionRequestedEventHandler(int From, int To, string movementInput, string attackInput);
    [Signal]
    public delegate void SpecialTransitionRequestedEventHandler(int From, int To, string name);

    //the character so that we can change velocity and things like that... maybe we just do this via signals??? 
    //actually yeah, definetly do this via signals...
    //but actually that seems bad because we're just adding additional calls each frame... 
    //let's just hold a reference to the enemy position and the camera.
    protected Character character;
    protected Character enemyCharacter;
    protected MeshInstance3D characterMesh;
    protected StandardMaterial3D baseColorMat = new StandardMaterial3D();

    protected BattleCamera currentBattleCamera;
    protected float characterMovementSpeed;

    // Animation player and tree for controlling animations
    protected AnimationPlayer animPlayer;
    protected AnimationTree animTree;
    protected AnimationNodeStateMachinePlayback animStateMachine;

    protected Label stateLabel;

    protected string cardinals = "2468";

    //use this to assign the actual state value itself
    //might just hard code this???
    [Export] public State state;

    //this method will be overriden by most states
    public abstract void Enter(string movementInput, string attackInput);

    //for the Idle state that can be entered with nothing pressed at all
    public virtual void Enter() { }

    public virtual void SpecialEnter(string name) { }

    public abstract void Exit();

    public virtual void Update(double delta)
    {
        character.leftSide = currentBattleCamera.Basis.X.Dot(-character.Basis.Z) > 0;
    }

    public virtual void PhysicsUpdate(double delta)
    {
        character.LookAt(enemyCharacter.GlobalPosition, Vector3.Up);
    }

    public abstract void HandleInput(string movementInput, string attackInput);

    public virtual void HandleSpecialInput(string specialInputName) { }

    public abstract void ForceSpecialInputTransition(string name, State targetState);

    public void SetAnimationNodes()
    {
        animPlayer = character.GetNode<AnimationPlayer>("%AnimationPlayer");
        animTree = character.GetNode<AnimationTree>("%AnimationTree");
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
    }

    public void SetDebugNodes()
    {
        stateLabel = character.GetNode<Label>("%stateLabel");
    }

    public void SetCharacter(Character currentCharacter)
    {
        character = currentCharacter;
        characterMovementSpeed = character.GetMovementSpeed();
        characterMesh = character.GetCharacterMesh();
        baseColorMat = baseColorMat = (StandardMaterial3D)characterMesh.GetSurfaceOverrideMaterial(0);
    }

    public void SetTargetAndCamera(Character targetCharacter, BattleCamera battleCamera)
    {
        enemyCharacter = targetCharacter;
        currentBattleCamera = battleCamera;
    }

    /// <summary>
    /// Plays a specified attack animation using the animation state machine.
    /// </summary>
    /// <param name="attackName">The name of the attack animation to play.</param>
    protected void PlayAttackAnimation(string attackName)
    {
        // Verify that Animation Tree and state machine are available:
        if (animTree == null || animStateMachine == null)
        {
            GD.PrintErr("Cannot play animation: animTree and/or animStateMachine is null!");
            return;
        }

        // Transition to specified attack state using playback controller
        try
        {
            animStateMachine.Travel(attackName);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to play animation {attackName}: {e.Message}");
        }
    }
}
