using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	[Library( "repairtool", Title = "Repair Tool" )]
	public partial class RepairTool : Carriable
	{
		public virtual float PrimaryRate => 2.0f;

		public override string ViewModelPath => "models/weapons/amhammer/amhammer_v.vmdl";

		public override void Spawn()
		{
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it

			SetModel( "models/weapons/amhammer/amhammer_w.vmdl" );
		}

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

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

		public virtual bool CanPrimaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;

			var rate = PrimaryRate;
			if ( rate <= 0 ) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		public virtual void AttackPrimary()
		{
			var player = Owner as FortwarsPlayer;
			foreach ( var tr in TraceHit( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 128f ) )
			{
				ViewModelEntity?.SetAnimBool( "hit", tr.Hit );

				if ( !tr.Hit )
					continue;

				if ( tr.Entity is FortwarsBlock block && block.TeamID == player.TeamID )
				{
					block.Heal( 10, tr.EndPos );
					continue;
				}

				tr.Entity.TakeDamage( DamageInfo.FromBullet( tr.EndPos, -tr.Normal * 10f, 10 ) );
			}

			ViewModelEntity?.SetAnimBool( "fire", true );
		}

		public virtual IEnumerable<TraceResult> TraceHit( Vector3 start, Vector3 end, float radius = 2.0f )
		{
			bool InWater = Physics.TestPointContents( start, CollisionLayer.Water );

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

		public override void SimulateAnimator( PawnAnimator anim )
		{
			anim.SetParam( "holdtype", 4 );
			anim.SetParam( "aimat_weight", 1.0f );
			anim.SetParam( "holdtype_handedness", 1 );
			anim.SetParam( "holdtype_pose_hand", 0.07f );
			anim.SetParam( "holdtype_attack", 1 );
		}
	}
}
