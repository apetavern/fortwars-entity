using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars;

partial class Grenade
{
	[Library( "fw_grenade_throwable", Title = "Grenade Throwable" )]
	partial class Throwable : BasePhysics
	{
		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/weapons/fraggrenade/fraggrenade_w.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		}

		public async Task ExplodeAfterSeconds( float seconds )
		{
			await Task.DelaySeconds( seconds );

			var sourcePos = Position;
			var radius = 256f;
			var overlaps = All.Where( e => Vector3.DistanceBetween( sourcePos, e.Position ) <= radius ).ToList();

			foreach ( var overlap in overlaps )
			{
				if ( overlap is not ModelEntity ent || !ent.IsValid() ) continue;
				if ( ent.LifeState != LifeState.Alive || !ent.PhysicsBody.IsValid() || ent.IsWorld ) continue;

				var dir = ( overlap.Position - Position ).Normal;
				var dist = Vector3.DistanceBetween( Position, overlap.Position );

				if ( dist > radius ) continue;

				var distanceFactor = 1.0f - Math.Clamp( dist / radius, 0, 1 );
				var force = distanceFactor * ent.PhysicsBody.Mass;
				force *= 0.5f; // Scale so it's not as strong

				if ( ent.GroundEntity != null )
				{
					ent.GroundEntity = null;
					if ( ent is Player { Controller: WalkController playerController } )
						playerController.ClearGroundEntity();
				}

				bool shouldDoDamage = false;

				if ( ent is not FortwarsPlayer player )
					shouldDoDamage = true;
				else if ( player.TeamID != ( Owner as FortwarsPlayer ).TeamID || Owner == player )
					shouldDoDamage = true;

				if ( shouldDoDamage )
					ent.TakeDamage( DamageInfoExtension.FromProjectile( 100 * distanceFactor, Position, Vector3.Up * 32, Client.Pawn ) );

				ent.ApplyAbsoluteImpulse( dir * force );
			}

			using ( Prediction.Off() )
			{
				var particle = Particles.Create( "particles/explosion/fw_explosion_base.vpcf", Position );
				PlaySound( "explosion.small" );
			}

			Delete();
		}
	}
}
