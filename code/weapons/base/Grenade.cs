// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.ComponentModel.DataAnnotations;

namespace Fortwars;

[Library( "fw_grenade", Title = "Grenade" )]
[Hammer.EditorModel( "models/fw_grenade.vmdl" )]
[Display( Name = "Grenade" )]
partial class Grenade : Carriable
{
	public static readonly Model WorldModel = Model.Load( "models/weapons/fraggrenade/fraggrenade_w.vmdl" );
	public override string ViewModelPath => "models/weapons/fraggrenade/fraggrenade_v.vmdl";

	public float PrimaryRate => 1.0f;
	public float SecondaryRate => 1.0f;
	public float ReloadTime => 1.0f;
	public int Count { get; private set; } = 3000;

	private TimeSince TimeSinceThrow { get; set; } = 0;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( CanThrow() )
			Throw();
	}

	public bool CanThrow()
	{
		return TimeSinceThrow > 1 && Input.Released( InputButton.Attack1 );
	}

	public void Throw()
	{
		TimeSinceThrow = 0;

		if ( Owner is not FortwarsPlayer player ) return;

		if ( Count <= 0 )
			return;

		Count--;

		// woosh sound
		// screen shake

		Rand.SetSeed( Time.Tick );

		if ( IsServer )
		{
			using ( Prediction.Off() )
			{
				var grenade = new Throwable
				{
					Position = Owner.EyePosition + Owner.EyeRotation.Forward * 3.0f,
					Owner = Owner
				};

				grenade.PhysicsBody.Velocity = Owner.EyeRotation.Forward * 600.0f + Owner.EyeRotation.Up * 200.0f + Owner.Velocity;

				grenade.CollisionGroup = CollisionGroup.Debris;
				grenade.SetInteractsExclude( CollisionLayer.Player );
				grenade.SetInteractsAs( CollisionLayer.Debris );

				_ = grenade.ExplodeAfterSeconds( 3.0f );
			}
		}

		player.SetAnimParameter( "fire", true );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HoldTypes.MeleePunch );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
