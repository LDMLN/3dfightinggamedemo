using Godot;
using System;

[GlobalClass]
public partial class Stage : Node3D
{
    MeshInstance3D forcefieldMesh;
    ShaderMaterial forcefieldShader;

    const float forcefieldLoweringSpeed = 10.0f;

    public override void _Ready()
    {
        forcefieldMesh = GetNode<MeshInstance3D>("%ForceField");
        forcefieldShader = (ShaderMaterial)forcefieldMesh.MaterialOverride;
    }

    public void BattleStart()
    {
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
        //we're overshooting the top value here (13 instead of 12) so we have a bit more time before we start seeing it lower.
        tween.TweenMethod(Callable.From((float TweenValue) => LowerForcefield(TweenValue)), 13.0, 0.0, forcefieldLoweringSpeed) ;
    }

    private void LowerForcefield(float tweenValue)
    {
        forcefieldShader.SetShaderParameter("dissolve_height", tweenValue);
    }

}
