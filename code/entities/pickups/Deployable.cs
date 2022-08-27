// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public partial class Deployable : Pickup
{
	[Net] public bool HasLanded { get; set; }
	private float ThrowSpeed => 100f;

	public override void Spawn()
	{
		base.Spawn();

		Predictable = true;
	}

	[Event.Tick]
	public void OnTick()
	{
		if ( HasLanded )
		{
			SetAnimParameter( "deployed", true );
			Rotation = Rotation.Angles().WithPitch( 0 ).ToRotation();
		}
		
		Move();
	}

	private void Move()
	{
		if ( !IsAuthority )
			return;
		
		var moveHelper = new MoveHelper( Position, Velocity );
		moveHelper.Trace = moveHelper.Trace.WorldOnly().Radius( 4f );

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

	public virtual void Resupply( FortwarsPlayer player )
	{

	}
}
