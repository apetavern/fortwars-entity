// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

partial class FortwarsGame
{
	public static void Explode( Vector3 position, Entity owner, float maxDamage = 100 )
	{
		Game.AssertServer();

		var sourcePos = position;
		var radius = 256f;
		var overlaps = All.Where( x => Vector3.DistanceBetween( sourcePos, x.Position ) <= radius ).ToList();

		foreach ( var overlap in overlaps )
		{
			if ( overlap is not ModelEntity ent || !ent.IsValid() ) continue;
			if ( ent.LifeState != LifeState.Alive || !ent.PhysicsBody.IsValid() || ent.IsWorld ) continue;

			var dir = ( overlap.Position - position ).Normal;
			var dist = Vector3.DistanceBetween( position, overlap.Position );

			if ( dist > radius ) continue;

			var distanceFactor = 1.0f - Math.Clamp( dist / radius, 0, 1 );
			var force = distanceFactor * ent.PhysicsBody.Mass;
			force *= 0.5f; // Scale so it's not as strong

			if ( ent.GroundEntity != null )
			{
				ent.GroundEntity = null;
				if ( ent is FortwarsPlayer { Controller: FortwarsWalkController playerController } )
					playerController.ClearGroundEntity();
			}

			bool shouldDoDamage = false;

			if ( ent is not FortwarsPlayer player )
				shouldDoDamage = true;
			else if ( player.TeamID != ( owner as FortwarsPlayer ).TeamID || owner == player )
				shouldDoDamage = true;

			var tr = Trace.Ray( position, ent.Position ).Run();
			if ( tr.Hit && tr.Entity != ent )
				continue;

			if ( shouldDoDamage )
			{
				ent.TakeDamage( DamageInfoExtension.FromProjectile( maxDamage * distanceFactor, position, Vector3.Up * 32, owner ) );
				ent.ApplyAbsoluteImpulse( dir * force );
			}
		}

		using ( Prediction.Off() )
		{
			var particle = Particles.Create( "particles/explosion/fw_explosion_base.vpcf", position );
			Sound.FromWorld( "explosion.small", position );
		}
	}
}
