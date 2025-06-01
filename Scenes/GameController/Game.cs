using Godot;
using System;

public partial class Game : Node3D
{
    private MainMenu mainMenu;
    public GameOver gameOver;

    public Character player1;
    public Character player2;
    public Enemy enemy;

    Node3D Battle;
    Node3D BattleVsEnemy;
    AudioStreamPlayer buttonSFX;

    private Hud hud;

    public override void _Ready()
    {
        base._Ready();
        //debug 
        GD.Print("This node (Game): ", GetPath());
        GD.Print("Tree root: ", GetTree().Root.GetPath());

        gameOver = GetNode<GameOver>("GameOver");
        gameOver.Hide();
        gameOver.PlayAgain += OnPlayAgain;

        buttonSFX = GetNode<AudioStreamPlayer>("ButtonSFX");
    }

    // Function to load a scene
    public Node SceneChange(PackedScene loadScene)
    {
        // clean up StartMenu
        var startMenu = GetNodeOrNull<Node>("StartMenu");
        if (startMenu != null)
        {
            startMenu.QueueFree();
            GD.Print("StartMenu removed.");
        }

        // add the new scene
        var scene = loadScene.Instantiate();
        //GetTree().Root.AddChild(scene);
        AddChild(scene);

        //debug
        GD.Print("This node (Game): ", GetPath());
        GD.Print("Tree root: ", GetTree().Root.GetPath());
        GD.Print("Current scene tree:");
        foreach (Node child in GetChildren())
        {
            GD.Print("- ", child.Name);
        }
        

        Battle = GetNodeOrNull<Node3D>("Battle");
        BattleVsEnemy = GetNodeOrNull<Node3D>("BattleVsEnemy");

        if (Battle != null)
        {
            SetupBattle(Battle);
        }
        else if (BattleVsEnemy != null)
        {
            SetupBattle(BattleVsEnemy);
        }
        else
        {
            GD.PrintErr("Game - Neither Battle nor BattleVSEnemy found");
        }

        return scene;
    }

    // function to quit game
    public void OnQuitGameBtnPressed()
    {
        // button sfx
        buttonSFX.Play();

        // quit functionality
        GetTree().Quit();
    }
    private void OnPlayAgain()
    {
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    // set up death signals and timeouts
    private void SetupBattle(Node battleNode)
    {
        player1 = battleNode.GetNode<Character>("Character");
        GD.Print($"Player1 set: {player1?.Name}");

        if (battleNode.HasNode("EnemyCharacter"))
        {
            player2 = battleNode.GetNode<Character>("EnemyCharacter");
            GD.Print($"EnemyCharacter found: {player2?.Name}");
            player2.CharacterDied += OnCharacterDied;
        }
        else if (battleNode.HasNode("Enemy"))
        {
            enemy = battleNode.GetNode<Enemy>("Enemy");
            GD.Print($"Enemy found: {enemy?.Name}");
            enemy.CharacterDied += OnCharacterDied;
        }

        player1.CharacterDied += OnCharacterDied;
        
        // Get Hud and connect signal
        hud = battleNode.GetNode<Hud>("HUD");

        if (hud != null)
        {
            hud.TimeUp += OnTimeUp;
        }
        else
        {
            GD.PrintErr("Hud node not found or null");
        }
    }

    private void OnTimeUp()
    {
        GD.Print("Game.cs detected timer expired.");

        GetTree().Paused = true;

        gameOver.Show();
    }

    private async void OnCharacterDied()
    {
        GD.Print("A character has died! Showing Game Over.");

        // wait one frame to ensure health bar updates
        await ToSignal(GetTree(), "process_frame");

        // Pause the game
        GetTree().Paused = true;

        gameOver.Show();
    }
}