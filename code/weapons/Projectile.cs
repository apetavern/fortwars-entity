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

		public override void Spawn()
		{
			base.Spawn();
		}

		[Event.Tick.Server]
		public void OnTick()
		{
			Velocity += Speed * Rotation.Forward * Time.Delta;
			Velocity += PhysicsWorld.Gravity * 0.5f * Time.Delta;

			Rotation = Rotation.LookAt( Velocity.Normal, Vector3.Up );

			var target = Position + Velocity * Time.Delta;
			var tr = Trace.Ray( Position, target ).Ignore( Owner ).Run();

			DebugOverlay.Line( tr.StartPos, tr.EndPos );

			if ( tr.Hit )
			{
				Explode( tr );
			}

			Position = target;
		}

		private void Explode( TraceResult tr )
		{
			if ( !IsServer )
				return;

			var sourcePos = tr.EndPos;
			var radius = 256f;
			var overlaps = Entity.All.Where( e => Vector3.DistanceBetween( sourcePos, e.Position ) <= radius ).ToList();

			foreach ( var overlap in overlaps )
			{
				if ( overlap is not ModelEntity ent || !ent.IsValid() ) continue;
				//if ( ent is FortwarsBlock && ent.PhysicsBody.BodyType == PhysicsBodyType.Static ) // Unfreeze block
				//	ent.PhysicsBody.BodyType = PhysicsBodyType.Dynamic;
				if ( ent.LifeState != LifeState.Alive || !ent.PhysicsBody.IsValid() || ent.IsWorld ) continue;

				var dir = (overlap.Position - tr.EndPos).Normal;
				var dist = Vector3.DistanceBetween( tr.EndPos, overlap.Position );

				if ( dist > radius ) continue;

				var distanceFactor = 1.0f - Math.Clamp( dist / radius, 0, 1 );
				var force = distanceFactor * ent.PhysicsBody.Mass;

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
					ent.TakeDamage( DamageInfo.Explosion( tr.EndPos, tr.Normal * 32, Weapon.WeaponAsset.MaxDamage * distanceFactor ) );

				ent.ApplyAbsoluteImpulse( dir * force );
			}

			using ( Prediction.Off() )
			{
				var particle = Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", Position );
				PlaySound( "rocket_jump" );
			}

			Delete();
		}
	}
}
