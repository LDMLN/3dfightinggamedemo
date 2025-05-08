using Godot;
using System;

public partial class HealthBar : ProgressBar
{
    public HealthBar healthBar;
    public ProgressBar DamageBar;
    public Timer timer;

    [Export]
    public int health {
        get; 
        set;
        }

    public override void _Ready()
    {
        healthBar = this;
        DamageBar = GetNode("DamageBar") as ProgressBar;
        timer = GetNode("Timer") as Timer;
    }
    public void InitHealth(int newHealth)
    {
        // initialize all bars to be completely full
        health = newHealth;
        healthBar.MaxValue = health;
        healthBar.Value = health;
        DamageBar.MaxValue = health;
        DamageBar.Value = health;
    }

    public void SetHealth(int newHealth)
    {
        int prevHealth = health;
        health = (int)Math.Min(healthBar.MaxValue, newHealth);
        healthBar.Value = health;

        if(health <= 0)
        {
            QueueFree(); // get rid of the progress bar
        }

        if (health < prevHealth)
        {
            timer.Start();
        }
        else
        {
            DamageBar.Value = health;
        }
    }

    public void OnTimerTimeout()
    {
        DamageBar.Value = health;
    }
}
