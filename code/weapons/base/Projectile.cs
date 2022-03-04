using Sandbox;
using System;
using System.Linq;

namespace Fortwars
{
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
			if ( !IsServer )
				return;

			var sourcePos = tr.EndPosition;
			var radius = 256f;
			var overlaps = Entity.All.Where( e => Vector3.DistanceBetween( sourcePos, e.Position ) <= radius ).ToList();

			foreach ( var overlap in overlaps )
			{
				if ( overlap is not ModelEntity ent || !ent.IsValid() ) continue;
				if ( ent.LifeState != LifeState.Alive || !ent.PhysicsBody.IsValid() || ent.IsWorld ) continue;

				var dir = (overlap.Position - tr.EndPosition).Normal;
				var dist = Vector3.DistanceBetween( tr.EndPosition, overlap.Position );

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
				else if ( player.TeamID != (Owner as FortwarsPlayer).TeamID || Owner == player )
					shouldDoDamage = true;

				if ( shouldDoDamage )
					ent.TakeDamage( DamageInfoExtension.FromProjectile( Weapon.WeaponAsset.MaxDamage * distanceFactor, Position, tr.Normal * 32, Client.Pawn ) );

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
