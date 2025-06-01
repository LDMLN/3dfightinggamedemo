using Godot;
using System;

public partial class Hud : CanvasLayer
{

    [Signal]
    public delegate void HealthDepletedEventHandler();
    [Signal]
    public delegate void TimeUpEventHandler();

    [Export]
    public int health;

    [Export]
    Timer timer;

    [Export]
    Label timerLabel;

    [Export]
    public Control startPopup;

    [Export]
    public Control endGamePopup;
    
    private float timeRemaining = 60f;
    private bool timerActive = true;

    

    public override void _Ready()
    {
        if (timerLabel == null)
            timerLabel = GetNode<Label>("TimeLabel");

        if (timer == null)
            timer = GetNode<Timer>("Timer");
        
        if (startPopup == null)
            startPopup = GetNode<Control>("StartPopup");

        if (endGamePopup == null)
            endGamePopup = GetNode<Control>("EndGamePopup");

        // Set up the timer to tick every second
        timer.WaitTime = 1.0f;
        timer.OneShot = false;
        timer.Timeout += OnTimerTimeout;
        
        // Initialize UI
        timerLabel.Show();
        startPopup.Show();
        endGamePopup.Hide();
        UpdateTimerLabel();

        // Show start popup briefly, then start timer
        GetTree().CreateTimer(2.0f).Timeout += StartCountdown;
    }

    private void StartCountdown()
    {
        startPopup.Hide();
        timerActive = true;
        timer.Start();
    }

    private void OnTimerTimeout()
    {
        if (!timerActive)
            return;

        timeRemaining -= 1f;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerActive = false;
            timer.Stop();
            ShowEndGamePopup();
        }

        UpdateTimerLabel();
    }

    private void UpdateTimerLabel()
    {
        timerLabel.Text = string.Format("{0:0}", timeRemaining);
    }

    private void ShowEndGamePopup()
    {
        endGamePopup.Show();
        GD.Print("Time's up!");
        EmitSignal(nameof(TimeUp));
    }
}
