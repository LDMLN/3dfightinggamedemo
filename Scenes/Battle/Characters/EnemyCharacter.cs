using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class EnemyCharacter : Character
{
    public override void _Ready()
    {
        base._Ready();
        //proof that the characters are working differently
        //movementSpeed = 10.0f;
        name = "Bobby";
        GD.Print(name);
        playerInputMethod = InputMethod.Joypad;
    }

}
