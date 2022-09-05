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

		SetAnimParameter( "jump", true );

		player.GroundEntity = null;
		player.Controller.GroundEntity = null;

		player.Velocity = ( player.EyeRotation.Forward * HorizontalSpeed ) + ( Rotation.Up * VerticalSpeed );//.WithZ( VerticalSpeed );
	}

	public override void SetLandedAppearance()
	{
		SetAnimParameter( "deployed", true );
		Rotation = Rotation.LookAt( Trace.Ray( Position + Vector3.Up, Position - Vector3.Up ).WithAnyTags( "solid", "playerclip" ).Radius( 4f ).Run().Normal, Vector3.Up ) * new Angles( 90, 0, 0 ).ToRotation();
	}

	public override void Move()
	{
		if ( !IsAuthority )
			return;

		var moveHelper = new MoveHelper( Position, Velocity );
		moveHelper.Trace = moveHelper.Trace.WorldOnly().WithAnyTags( "solid", "playerclip" );

		moveHelper.Velocity += ThrowSpeed * Rotation.Forward * Time.Delta;
		moveHelper.Velocity += Vector3.Down * 800f * Time.Delta;

		bool grounded = moveHelper.TraceDirection( Vector3.Down ).Hit;
		if ( grounded )
		{
			moveHelper.ApplyFriction( 8.0f, Time.Delta );
			HasLanded = true;
		}

		moveHelper.TryMove( Time.Delta );

		Position = moveHelper.Position;
		Velocity = moveHelper.Velocity;
	}
}
