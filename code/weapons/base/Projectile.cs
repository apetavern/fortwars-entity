// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System;
using System.Linq;

namespace Fortwars;

public class Projectile : ModelEntity
{
	public float Speed { get; set; }
	private Vector3 Forward { get; set; }

	public FortwarsWeapon Weapon;

	Particles trailParticle;

	public override void Spawn()
	{
		base.Spawn();

	}

	[Event.Tick.Server]
	public void OnTick()
	{
		if ( trailParticle == null )
		{
			trailParticle = Particles.Create( "particles/rpg/fw_rpg_projectile_fire.vpcf" );
			trailParticle.SetPosition( 0, GetAttachment( "trail" ).Value.Position ); //Can't parent the particle or it gets destroyed with the projectile.
		}

		trailParticle.SetPosition( 0, GetAttachment( "trail" ).Value.Position ); //Have to keep positioning it...

		Velocity += Speed * Rotation.Forward * Time.Delta;
		Velocity += Map.Physics.Gravity * 0.5f * Time.Delta;

		Rotation = Rotation.LookAt( Velocity.Normal, Vector3.Up );

		var target = Position + Velocity * Time.Delta;
		var tr = Trace.Ray( Position, target ).Ignore( Owner ).Run();

		if ( tr.Hit )
		{
			trailParticle.Destroy( false );//Destroy it when it's done.
			Explode( tr );
		}

		Position = target;
	}

	private void Explode( TraceResult tr )
	{
		Game.Explode( tr.EndPosition, Owner, Weapon.WeaponAsset.MaxDamage );

		Delete();
	}
}
