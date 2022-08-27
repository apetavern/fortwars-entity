// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

/// <summary>
/// Players capture the flag in this area.
/// </summary>
[Library( "func_spawn_area" )]
[SandboxEditor.Solid]
[SandboxEditor.RenderFields]
[SandboxEditor.VisGroup( SandboxEditor.VisGroup.Dynamic )]
public partial class FuncSpawnArea : BrushEntity
{
    [Property]
    public Team Team { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		Tags.Add( "trigger" );
		Tags.Add( Team == Team.Red ? "redteam" : "blueteam" );//Should make it so opposite teams can't enter eachother's spawn.
        EnableSolidCollisions = false;
        EnableTouch = true;

        Transmit = TransmitType.Never;
    }

    public override void StartTouch( Entity other )
    {
        base.StartTouch( other );

        if ( other.IsWorld )
            return;

        if ( other is FortwarsPlayer player && player.TeamID != Team )
            other.TakeDamage( DamageInfo.Generic( 10000f ) );
    }
}
