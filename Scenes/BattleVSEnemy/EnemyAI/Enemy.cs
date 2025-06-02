using Godot;
using System;

public partial class Enemy : Character
{
    public override void _Ready()
    {
        base._Ready();

        // Sync hit/hurt collision masks if enemyCharacter is set
        if (enemyCharacter != null)
        {
            // Get this character's own boxes
            var myHitBox = GetHitBox();
            var myHurtBox = GetHurtBox();

            // Get opponent's boxes
            var enemyHitBox = enemyCharacter.GetHitBox();
            var enemyHurtBox = enemyCharacter.GetHurtBox();

            if (myHitBox != null && enemyHurtBox != null)
            {
                // My hitbox should detect the enemy's hurt layer
                myHitBox.CollisionMask = enemyHurtBox.CollisionLayer;
                GD.Print($"{Name}'s HitBox mask layer set to {enemyHurtBox.CollisionLayer} (enemy's HurtBox layer)");
            }

            if (myHurtBox != null && enemyHitBox != null)
            {
                // My hurtbox should be hit by the enemy's hitbox
                myHurtBox.CollisionLayer = enemyHitBox.CollisionMask;
                GD.Print($"{Name}'s HurtBox layer set to {enemyHitBox.CollisionMask} (enemy's HitBox mask layer)");
            }
        }
        else
        {
            GD.Print($"{Name}: enemyCharacter not set. Cannot sync collision layers.");
        }
    }
}
