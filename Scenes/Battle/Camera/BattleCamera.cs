using Godot;
using System;
using System.Reflection.Metadata;

[GlobalClass]
public partial class BattleCamera : Camera3D
{
    [ExportCategory("Camera Settings")]
    [Export] public float cameraDistance = 4.0f;
    [Export] public float cameraElevation = 2.5f;
    //this should be fast or else things might feel disjointed
    [Export] public float cameraMoveSpeed = .5f;
    [Export] public float cameraMinZoom = 2.5f;
    [Export] public float cameraMaxZoom = 8.5f;
    [Export] public Vector2 margin = new Vector2(200, 100);

    Vector2 screenSize;
    


    Character targetPlayer1;
    Character targetPlayer2;

    Vector3 lookPosition;
    Vector3 placementPosition;

    MeshInstance3D MidpointSphere;
    MeshInstance3D PerpendendicularSphere;

    Node3D cameraPivot;



    public override void _Ready()
    {
        screenSize = GetViewport().GetVisibleRect().Size;
        GD.Print("screenSize: " + screenSize);
        MidpointSphere = GetNode<MeshInstance3D>("DebugSphere1");
        PerpendendicularSphere = GetNode<MeshInstance3D>("DebugSphere2");
    }


    public override void _Process(double delta)
    {
        //TODO: don't like having this check every frame... let's rewrite this so we know it also has two targets.
        //but wait... actually maybe we do want this to be here because if a character is ringed out then we don't want it to follow them down...
        if (targetPlayer1 is null || targetPlayer2 is null)
        {
            GD.Print("returning cuz targets are null: Camera");
            return;
        }

        //could do with some cleaning up
        lookPosition = (targetPlayer1.GlobalPosition + targetPlayer2.GlobalPosition) /2;
        Vector3 directionBetweenPlayers = targetPlayer1.GlobalPosition.DirectionTo(targetPlayer2.GlobalPosition);
        //technically, since directionBetweenPlayers is normalized from DirectionTo and Vector3.Up is a unit vector,
        //the cross vector should also (probably) already be normalized... but just to be safe.
        Vector3 cameraToLookDirection = directionBetweenPlayers.Cross(Vector3.Up).Normalized();

        float zoomDistance = targetPlayer1.GlobalPosition.DistanceTo(targetPlayer2.GlobalPosition) / 2;
        zoomDistance = Math.Clamp(zoomDistance, cameraMinZoom, cameraMaxZoom);
        cameraElevation = zoomDistance / 2;
        Vector3 cameraOffsetVector = cameraToLookDirection * zoomDistance;

        //for debugging
        //MidpointSphere.GlobalPosition = lookPosition;
        //this is wrong...handle the placement here...
        GlobalPosition = lookPosition + cameraOffsetVector + Vector3.Up * cameraElevation;
        //maybe we'll lerp... not sure about game feel here
        //GlobalPosition = GlobalPosition.Lerp(lookPosition + cameraToLookDirection * cameraDistance + Vector3.Up * cameraElevation, cameraMoveSpeed);
        //
        LookAt(lookPosition);
    }


    //might have the camera just do this itself if we put both the players in a group...
    public void SetPlayers(Character player1, Character player2)
    {
        targetPlayer1 = player1;
        targetPlayer2 = player2;
    }
}
