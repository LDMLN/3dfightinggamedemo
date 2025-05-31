using Godot;
using System;

public partial class HealthBar : ProgressBar
{
    public Timer Timer;
    public ProgressBar DamageBar;
    public Character Player;
    public int _health = 10;

    // defines a public property instead of directly exposing the _health variable 
    // and adds logic to run when the value is set.
    public int Health
    {
        get => _health;
        set => SetHealth(value);
    }

    public override void _Ready()
    {
        Timer = GetNode<Timer>("Timer");
        Timer.WaitTime = 0.4f;
        Timer.OneShot = true;
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

    public void SetPlayer(Character _player)
    {
        Player = _player;
        Player.HealthChanged += OnHealthChanged;
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
    
    private void OnHealthChanged(float healthRatio)
    {
        int newHealth = Mathf.RoundToInt(healthRatio * (int)MaxValue);
        SetHealth(newHealth);
    }
}
