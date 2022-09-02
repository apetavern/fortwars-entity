// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using SandboxEditor;

namespace Fortwars;

[Library( "func_wall_toggle" )]
[Solid]
[RenderFields]
[VisGroup( VisGroup.Dynamic )]
[HammerEntity]
public partial class FuncWallToggle : BrushEntity
{
    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel( PhysicsMotionType.Keyframed ); 
        EnableAllCollisions = true;
        EnableTouch = true;
    }

    public void Show()
    {
        // TODO: Why does changing EnableAllCollisions do fuck all
        EnableAllCollisions = true;
        EnableDrawing = true;
    }

    public void Hide()
    {
        // TODO: Ditto
        EnableAllCollisions = false;
        EnableDrawing = false;
    }
}
