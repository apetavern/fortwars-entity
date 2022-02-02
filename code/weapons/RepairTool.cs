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
			(ViewModelEntity)?.SetAnimBool( "fire", true );
			(ViewModelEntity)?.SetAnimBool( "hit", true );

			foreach ( var tr in TraceHit( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 72f ) )
			{
				DebugOverlay.Sphere( tr.EndPos, 4f, Color.Green, false, 5f );

				if ( !tr.Hit )
					return;


				if ( tr.Entity is FortwarsBlock block )
				{
					if ( block.TeamID == (Owner as FortwarsPlayer).TeamID )
					{
						block.Heal( 10, tr.EndPos );
						continue;
					}
				}

				tr.Entity.TakeDamage( DamageInfo.Generic( 10 ) );
			}
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
	}
}
