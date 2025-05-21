using Godot;
using System;

public partial class GameScreen : CanvasLayer
{
    [Signal]
    public delegate void HealthDepletedEventHandler();

    [Export]
    public int health;

    [Export]
    Timer timer;

    [Export]
    Label timerLabel;

    

    public override void _Ready()
    {
        timerLabel = GetNode<Label>("TimerLabel");
        timer = GetNode<Timer>("Timer");

        timer.WaitTime = 60.0f;

        timerLabel.Show();
        
        
        timer.Timeout += OnTimerTimeout;
        timer.Start();
    }

    public override void _Process(double delta)
    {
        timerLabel.Text = string.Format("{0:0.00}", timer.TimeLeft); // Display time with two decimal places
    }

    private void OnTimerTimeout()
    {
        //TO DO
    }
}
