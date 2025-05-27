using Godot;
using System;

public partial class HealthBar : ProgressBar
{
    [Export]
    public Timer Timer;

    [Export]
    public ProgressBar DamageBar;

    [Export]
    public Character Player;

    [Export]
    public int _health = 0;

    // defines a public property instead of directly exposing the _health variable 
    // and adds logic to run when the value is set.
    public int Health
    {
        get => _health;
        set => SetHealth(value);
    }

    public override void _Ready()
    {
        // Optionally get references if not assigned via Export
        if (Timer == null)
            Timer = GetNode<Timer>("Timer");

        if (DamageBar == null)
            DamageBar = GetNode<ProgressBar>("DamageBar");

        Timer.Timeout += OnTimerTimeout;
    }

    private void SetHealth(int newHealth)
    {
        int prevHealth = _health;
        _health = Mathf.Min((int)MaxValue, newHealth);
        Value = _health;

        if (_health < 0)
        {
            QueueFree();
        }

        if (_health < prevHealth)
        {
            Timer.Start();
        }
        else
        {
            DamageBar.Value = _health;
        }
    }

    public void InitHealth(int initialHealth)
    {
        _health = initialHealth;
        MaxValue = initialHealth;
        Value = initialHealth;

        DamageBar.MaxValue = initialHealth;
        DamageBar.Value = initialHealth;
    }

    private void OnTimerTimeout()
    {
        DamageBar.Value = _health;
    }
}
