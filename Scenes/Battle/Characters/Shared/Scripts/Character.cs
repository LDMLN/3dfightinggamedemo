using Godot;
using System;

//author: Luiz Leao
[GlobalClass]
public partial class Character : CharacterBody3D
{
    //this has to be passed into it from the the Battle scene
    private Camera3D battleCamera;
    [Export] public CharacterBody3D enemyCharacter;

    float angleToEnemy = 0.0f;
    float movementSpeed = 5.0f;
    Vector3 velocity = new(0.0f, 0.0f, 0.0f);

    public override void _Process(double delta)
    {
        //might not need anything now... or might have some
        //kind of "translator" function here...
    }

    public override void _PhysicsProcess(double delta)
    {
        //radius of the circle we'll be moving in
        float distanceToEnemy = this.GlobalPosition.DistanceTo(enemyCharacter.GlobalPosition);
        angleToEnemy += (float)(movementSpeed * delta);
        //implement the 
        LookAt(enemyCharacter.GlobalPosition, Vector3.Up);

        //update velocity here?

        Velocity = velocity.Rotated(new Vector3(0, 1, 0), Rotation.Y);
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        
    }

    public void SetCamera(Camera3D stageCamera)
    {
        battleCamera = stageCamera;
    }

    public void SetEnemyCharacter(Character enemy)
    {
        enemyCharacter = enemy;
    }

}
