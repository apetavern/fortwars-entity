using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	[Library( "bogroll", Title = "Bogroll Weapon" )]
	public partial class BogRoll : Carriable
	{
		[Net] public Team Team { get; set; }

		public virtual float PrimaryRate => 2.0f;

		public override string ViewModelPath => "models/items/bogroll/bogroll_v.vmdl";

		public override void Spawn()
		{
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it

			SetModel( "models/items/bogroll/bogroll_w.vmdl" );
		}

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

		[Net] bool RespawntimerStarted { get; set; }
		[Net] TimeSince TimeSinceDropped { get; set; }

		[Net] bool CantPickup { get; set; }

		public override void OnCarryStart( Entity carrier )
		{
			carrier.Inventory.SetActive( this );

			if (Team == Team.Blue && Game.Instance.BlueFlagCarrier != carrier as FortwarsPlayer )
			{
				Game.Instance.PlayerPickupEnemyFlagFloor( carrier as FortwarsPlayer );
			}

			if ( Team == Team.Red && Game.Instance.BlueFlagCarrier != carrier as FortwarsPlayer )
			{
				Game.Instance.PlayerPickupEnemyFlagFloor( carrier as FortwarsPlayer );
			}

			base.OnCarryStart( carrier );
		}

		public override bool CanCarry( Entity carrier )
		{
			return !CantPickup && (carrier as FortwarsPlayer).TeamID != Team;
		}

		[Event.Tick.Server]//Server tick so the bogroll can keep track of drop time.
		public void Tick()
		{
			if ( Parent != null && Parent.ActiveChild != this )
			{
				CantPickup = true;
				Game.Instance.PlayerDropFlag( Owner as FortwarsPlayer );
				( Parent as FortwarsPlayer).Inventory.Drop( this );
			}
			else if( Parent != null && Parent.ActiveChild == this )
			{
				RespawntimerStarted = false;
			}

			if ( Parent == null && !RespawntimerStarted )
			{
				TimeSinceDropped = 0;
				RespawntimerStarted = true;
			}

			if ( RespawntimerStarted && TimeSinceDropped > 15f )
			{
				Game.Instance.ReturnFlag( Team );
				Delete();
			}
		}

		public override void EndTouch( Entity other )
		{
			CantPickup = false;
			base.EndTouch( other );
		}

		public override void Simulate( Client player )
		{
			if ( !Owner.IsValid() )
				return;

			switch ( Team )
			{
				case Team.Invalid:
					break;
				case Team.Red:
					SetMaterialGroup( 1 );
					break;
				case Team.Blue:
					SetMaterialGroup( 0 );
					break;
				default:
					break;
			}


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
			player.SetAnimBool( "b_attack", true );
			foreach ( var tr in TraceHit( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 128f ) )
			{
				ViewModelEntity?.SetAnimBool( "hit", tr.Hit );

				if ( !tr.Hit )
				{
					MissEffects();
					continue;
				}

				HitEffects();

				/*if ( tr.Entity is FortwarsBlock block && block.TeamID == player.TeamID )
				{
					block.Heal( 10, tr.EndPos );
					continue;
				}*/

				tr.Entity.TakeDamage( DamageInfo.FromBullet( tr.EndPos, -tr.Normal * 10f, 10 ) );
			}

			ViewModelEntity?.SetAnimBool( "fire", true );
		}

		[ClientRpc]
		private void MissEffects()
		{
			_ = new Sandbox.ScreenShake.Perlin( 1.0f, 0.1f, 4.0f, 1.0f );
		}

		[ClientRpc]
		private void HitEffects()
		{
			_ = new Sandbox.ScreenShake.Perlin( 0.25f, 4.0f, 4.0f, 0.5f );
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
