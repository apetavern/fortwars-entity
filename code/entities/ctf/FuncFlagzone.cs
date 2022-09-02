// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using SandboxEditor;

namespace Fortwars;

/// <summary>
/// Players capture the flag in this area.
/// </summary>
[Library( "func_flagzone" )]
[Solid]
[RenderFields]
[VisGroup( VisGroup.Dynamic )]
[HammerEntity]
public partial class FuncFlagzone : BrushEntity
{
	[Property]
	public Team Team { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel( PhysicsMotionType.Static );
		Tags.Add( "trigger" );
		Tags.Add( Team == Team.Red ? "blueteam" : "redteam" );//Opposite teams because the collision matrix has them set up as trigger for their own team
		EnableSolidCollisions = false;
		EnableTouch = true;

		Transmit = TransmitType.Never;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other.IsWorld )
			return;

		if ( Game.Instance.Round is not CombatRound )
			return;

		if ( other is Player )
			Game.Instance.OnPlayerTouchFlagzone( other as FortwarsPlayer, Team );
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );
	}
}
