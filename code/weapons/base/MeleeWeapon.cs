using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	public partial class MeleeWeapon : Carriable
	{
		public virtual float PrimaryRate => 2.0f;
		[Net, Predicted] public TimeSince TimeSincePrimaryAttack { get; set; }

		public override void Simulate( Client player )
		{
			if ( !Owner.IsValid() )
				return;

			if ( CanPrimaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSincePrimaryAttack = 0;
					AttackPrimary();
				}
			}
		}

		public virtual IEnumerable<TraceResult> TraceHit( Vector3 start, Vector3 end, float radius = 2.0f )
		{
			bool InWater = Map.Physics.IsPointWater( start );

			var tr = Trace.Ray( start, end )
					.UseHitboxes()
					.HitLayer( CollisionLayer.Water, !InWater )
					.HitLayer( CollisionLayer.Debris )
					.Ignore( Owner )
					.Ignore( this )
					.Size( radius )
					.Run();

			yield return tr;
		}

		public virtual void AttackPrimary()
		{
			var player = Owner as FortwarsPlayer;
			player.SetAnimParameter( "b_attack", true );
			ViewModelEntity?.SetAnimParameter( "fire", true );

			foreach ( var tr in TraceHit( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 128f ) )
			{
				if ( !tr.Hit )
					continue;

				tr.Entity.TakeDamage( DamageInfo.FromBullet( tr.EndPosition, -tr.Normal * 10f, 50 ) );
			}
		}
		public virtual bool CanPrimaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;

			var rate = PrimaryRate;
			if ( rate <= 0 ) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}
	}
}
