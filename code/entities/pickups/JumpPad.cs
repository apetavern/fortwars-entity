// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public partial class JumpPad : Deployable
{
	protected override float TimeBetweenResupplies => 2;
	protected override float ResupplyRadius => 32f;
	protected override bool CreateParticles => false;

	private float VerticalSpeed => 768f;
	private float HorizontalSpeed => 512f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/items/jumppad/jumppad.vmdl" );

		Components.RemoveAny<BobbingComponent>();
	}

	public override void Resupply( FortwarsPlayer player )
	{
		Log.Trace( $"Jumping player {player}" );

		player.GroundEntity = null;
		player.Controller.GroundEntity = null;

		player.Velocity = (player.EyeRotation.Forward * HorizontalSpeed).WithZ( VerticalSpeed );
	}
}
