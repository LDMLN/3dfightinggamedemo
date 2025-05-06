using Godot;
using System;

[GlobalClass]
public partial class Stage : Node3D
{
    MeshInstance3D forcefieldMesh;
    ShaderMaterial forcefieldShader;

    StaticBody3D stageBoundary;

    const float forcefieldLoweringSpeed = 7.0f;

    public override void _Ready()
    {
        forcefieldMesh = GetNode<MeshInstance3D>("%ForceField");
        forcefieldShader = (ShaderMaterial)forcefieldMesh.MaterialOverride;
        stageBoundary = GetNode<StaticBody3D>("StageBoundary");
    }

    public void BattleStart()
    {
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
        //we're overshooting the top value here (-.3 instead of 0) so we have a bit more time before we start seeing it lower.
        tween.TweenMethod(Callable.From((float TweenValue) => LowerForcefield(TweenValue)), -.1, 0.9, forcefieldLoweringSpeed) ;
    }

    private void LowerForcefield(float tweenValue)
    {
        forcefieldShader.SetShaderParameter("dissolve_cutoff", tweenValue);
    }

}
