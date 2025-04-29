using Godot;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

//author: Luiz Leao
[GlobalClass]
public partial class Character : CharacterBody3D
{
    //this has to be passed into it from the the Battle scene
    private Camera3D battleCamera;
    [Export] public Character enemyCharacter;
    [Export(PropertyHint.Range, "1,2")] public int whichPlayer;

    float angleToEnemy = 0.0f;
    float movementSpeed = 5.0f;
    Vector3 velocity = Vector3.Zero;
    Vector3 targetPosition = Vector3.Zero;

    // Reference to the Imported Model's Armature and Skeleton3D
    private Node3D armature;
    private Skeleton3D skeleton;

    // Animation player and tree for controlling animations
    private AnimationPlayer animPlayer;
    private AnimationTree animTree;

    // StandardMaterial3D baseColorMat = new StandardMaterial3D();
    // StandardMaterial3D attackRedMat = new StandardMaterial3D() { AlbedoColor = new Color(1.0f, 0.0f, 0.0f) };
    // private MeshInstance3D characterMesh = new();

    //didn't end up needing
    //string[] movementInputs = {"Walk_Right", "Walk_Left", "Walk_Up", "Walk_Down"};
    //string[] attackInputs = {"Punch", "Heavy_Punch", "Kick", "Heavy_Kick"};

    //this is the center of the player model, used for camera since model transform is currently set at the feet.
    private Node3D characterCenter;

    string cardinals = "2468";

    public override void _Ready()
    {
        // characterMesh = GetNode<MeshInstance3D>("MeshInstance3D");
        // baseColorMat = (StandardMaterial3D)characterMesh.GetSurfaceOverrideMaterial(0);

        // Get references to the new structure components
        armature = GetNode<Node3D>("Armature");
        skeleton = GetNode<Skeleton3D>("Armature/Skeleton3D");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animTree = GetNode<AnimationTree>("AnimationTree");
        characterCenter = GetNode<Node3D>("%CharacterCenter");
    }

    /*
    public override void _Process(double delta)
    {

    }
    */

    public override void _PhysicsProcess(double delta)
    {
        LookAt(enemyCharacter.GlobalPosition, Vector3.Up);
        //not handling player2 at this moment but I want it to have this script
        if (whichPlayer == 2)
        {
            return;
        }

        // Check if we're not moving and set idle animation
        if (velocity.LengthSquared() < 0.01f)
        {
            if (animTree != null && animTree.Active)
            {
                animTree.Set("parameters/Locomotion/blend_position", Vector2.Zero);
            }
        }

        //radius of the circle we'll be moving in
        //float distanceToEnemy = this.GlobalPosition.DistanceTo(enemyCharacter.GlobalPosition);
        angleToEnemy += (float)(movementSpeed * delta);

        Velocity = new Vector3(velocity.X, 0, velocity.Z);

        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        //not handling player2 at this moment but I want it to have this script
        if (whichPlayer == 2)
        {
            return;
        }

        velocity = Vector3.Zero;
        if (@event is InputEventMouseMotion)
        {
            return;
        }

        //handle through state machine later.
        string[] newInputs = TranslateInput();
        HandleNewInput(newInputs[0], newInputs[1]);

        // After handling input, if we're in "neutral" (5), set to idle
        if (newInputs[0] == "5" && newInputs[1] == "")
        {
            if (animTree != null && animTree.Active)
            {
                animTree.Set("parameters/Locomotion/blend_position", Vector2.Zero);
            }
        }

        GD.Print(newInputs);
    }

    private void HandleNewInput(string movementInput, string attackInput)
    {
        if (attackInput != "")
        {
            //almost certainly going to be a state in the statemachine...
            HandleAttackInput(movementInput, attackInput);
        }
        //we don't move if we're attacking
        else if (cardinals.Contains(movementInput))
        {
            HandleCardinalMovement(movementInput);
        }
        else
        {
            HandleNonCardinalMovement(movementInput);
        }
    }

    private async void HandleAttackInput(string movementInput, string attackInput)
    {
        if (movementInput != "5")
        {
            GD.Print("Command Normal");
        }
        else
        {
            if (attackInput == "P")
            {
                GD.Print("Punch!");
            }
            else if (attackInput == "HP")
            {
                GD.Print("Heavy Punch!");
            }
            else if (attackInput == "K")
            {
                GD.Print("Kick!");
            }
            else
            {
                GD.Print("Heavy Kick!");
            }
        }

        // characterMesh.SetSurfaceOverrideMaterial(0, attackRedMat);
        // await ToSignal(GetTree().CreateTimer(.15), "timeout");
        // characterMesh.SetSurfaceOverrideMaterial(0, baseColorMat);
    }


    private void HandleCardinalMovement(string movementInput)
    {
        float horizontalSpeed = 0;
        float strafeSpeed = 0;
        Vector2 blendPosition = Vector2.Zero;

        //this is cleaner, imo, but might be a problem when trying to handle things like (if "8" is held, we jump) later so I'm not using it... might use later though.
        //strafeSpeed = movementInput == "8" ? 1 : movementInput == "2" ? -1 : 0;
        //horizontalSpeed = movementInput == "6" ? 1 : movementInput == "4" ? -1 : 0;

        //if held, jump up
        if (movementInput == "8")
        {
            strafeSpeed = 1;
            blendPosition = new Vector2(-1, 0); // Left in BlendSpace
        }
        //if held, crouch
        else if (movementInput == "2")
        {
            strafeSpeed = -1;
            blendPosition = new Vector2(1, 0); // Right in BlendSpace
        }
        else if (movementInput == "6")
        {
            horizontalSpeed = 1;
            blendPosition = new Vector2(0, 1); // Forward in BlendSpace
        }
        else if (movementInput == "4")
        {
            horizontalSpeed = -1;
            blendPosition = new Vector2(0, -1); // Backward in BlendSpace
        }
        else
        {
            GD.Print("STOP MOVING!!!!");
            horizontalSpeed = 0;
            strafeSpeed = 0;
            blendPosition = Vector2.Zero; // Idle position in BlendSpace
        }

        // Apply the blend position to the animation tree
        if (animTree != null && animTree.Active)
        {
            animTree.Set("parameters/Locomotion/blend_position", blendPosition);
        }

        //we will only be moving either forward/backward or "up"/"down"
        horizontalSpeed = strafeSpeed == 0 ? horizontalSpeed : 0;
        strafeSpeed = horizontalSpeed == 0 ? strafeSpeed : 0;

        Vector3 directionToEnemy = GlobalPosition.DirectionTo(enemyCharacter.GlobalPosition);

        if (horizontalSpeed != 0)
        {
            velocity += directionToEnemy * horizontalSpeed * movementSpeed;
        }
        //this has two bugs:
        //1.) the initial movement goes in a straight line and then rounds out to a curve...
        //2.) the curve gets wider if looped many times around the enemy character
        if (strafeSpeed != 0)
        {
            Vector3 strafeTarget = directionToEnemy.Rotated(Vector3.Up, (float)(Math.PI / 2));
            velocity = strafeTarget.Normalized() * movementSpeed * strafeSpeed;
            velocity.Y = 0;
        }
        //check to make sure everything doesn't get out of control.
        velocity = velocity.LimitLength(movementSpeed);
        velocity = (horizontalSpeed == 0 && strafeSpeed == 0) ? Vector3.Zero : velocity;
    }

    private void HandleNonCardinalMovement(string movementInput)
    {
        if (movementInput == "9")
        {
            GD.Print("SMALL FORWARD HOP");
        }
        else if (movementInput == "7")
        {
            GD.Print("SMALL BACK HOP");
        }
        else if (movementInput == "1")
        {
            GD.Print("CROUCH");
        }
        else if (movementInput == "3")
        {
            GD.Print("CROUCH AND MOVE FORWARD");
        }
    }

    private void Crouch()
    {
        //implement crouch
    }


    private static string[] TranslateInput()
    {
        string movementInput = GetMovementInput();
        string attackInput = GetAttackInput();
        //we are building the movement string.
        return [movementInput, attackInput];
    }


    private static string GetMovementInput()
    {
        string movementInput = "";
        //gets whether we are moving up, down, left or right or if opp directions pushed simultaneously
        float depthAxis = Input.GetAxis("Walk_Down", "Walk_Up");
        float horizontalAxis = Input.GetAxis("Walk_Left", "Walk_Right");

        bool up = depthAxis > 0;
        bool down = depthAxis < 0;
        bool right = horizontalAxis > 0;
        bool left = horizontalAxis < 0;

        if (up && right)
        {
            movementInput = "9";
        }
        else if (up && left)
        {
            movementInput = "7";
        }
        else if (down && right)
        {
            movementInput = "3";
        }
        else if (down && left)
        {
            movementInput = "1";
        }
        else if (up)
        {
            movementInput = "8";
        }
        else if (down)
        {
            movementInput = "2";
        }
        else if (right)
        {
            movementInput = "6";
        }
        else if (left)
        {
            movementInput = "4";
        }

        movementInput = movementInput == "" ? "5" : movementInput;

        return movementInput;
    }

    private static string GetAttackInput()
    {
        string attackInput = "";
        if (Input.IsActionJustPressed("Punch"))
        {
            attackInput += "P";
        }
        if (Input.IsActionJustPressed("Heavy_Punch"))
        {
            attackInput += "HP";
        }
        if (Input.IsActionJustPressed("Kick"))
        {
            attackInput += "K";
        }
        if (Input.IsActionJustPressed("Heavy_Kick"))
        {
            attackInput += "HK";
        }

        return attackInput;
    }


    public void SetCamera(Camera3D stageCamera)
    {
        battleCamera = stageCamera;
    }

    public void SetEnemyCharacter(Character enemy)
    {
        enemyCharacter = enemy;
    }

    public Node3D GetCharacterCenter()
    {
        return characterCenter;
    }

}
