using Sandbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
		[Net] public TimeSince TimeSinceDropped { get; set; }

		[Net] bool CantPickup { get; set; }

		[Net] RollReturnZone ReturnZone { get; set; }

		public override void OnCarryStart( Entity carrier )
		{
			if ( carrier is FortwarsPlayer player )
			{
				player.Inventory.SetActive( this );

				if ( Team == Team.Blue && Game.Instance.BlueFlagCarrier != player )
				{
					Game.Instance.PlayerPickupEnemyFlagFloor( player );
				}

				if ( Team == Team.Red && Game.Instance.BlueFlagCarrier != player )
				{
					Game.Instance.PlayerPickupEnemyFlagFloor( player );
				}

				base.OnCarryStart( carrier );
			}

		}

		public override bool CanCarry( Entity carrier )
		{
			return !CantPickup && (carrier as FortwarsPlayer).TeamID != Team;
		}

		public override void TakeDamage( DamageInfo info )
		{
			base.TakeDamage( info );
		}

		protected override void OnDestroy()
		{
			if ( IsServer )
			{
				Game.Instance.ReturnFlag( Team );

				if ( ReturnZone != null )
					ReturnZone.Delete();
			}

			base.OnDestroy();
		}

		// Server tick so the bogroll can keep track of drop time.
		[Event.Tick.Server]
		public void Tick()
		{
			if ( Parent != null && (Parent as FortwarsPlayer).ActiveChild != this )
			{
				ThrowRoll();
			}
			else if ( Parent != null && (Parent as FortwarsPlayer).ActiveChild == this )
			{
				RespawntimerStarted = false;

				if ( ReturnZone != null )
					ReturnZone.Delete();
			}

			if ( Parent == null && !RespawntimerStarted )
			{
				TimeSinceDropped = 0;
				RespawntimerStarted = true;
				ReturnZone = new RollReturnZone();
				ReturnZone.AttachToRoll( this );
			}

			if ( RespawntimerStarted && TimeSinceDropped > 15f )
			{
				PlaySound( "enemyflagreturned" );//Play sad flag return sounds

				Delete();
			}
		}

		[Event.Tick.Client]
		public void ClientTick()
		{
			if ( RespawntimerStarted )
			{
				switch ( Team )
				{
					case Team.Invalid:
						break;
					case Team.Red:
						DebugOverlay.Text( CollisionWorldSpaceCenter + Vector3.Up * 20f, "" + MathF.Ceiling( 15f - TimeSinceDropped ), Color.Red, 0, 500 );
						break;
					case Team.Blue:
						DebugOverlay.Text( CollisionWorldSpaceCenter + Vector3.Up * 20f, "" + MathF.Ceiling( 15f - TimeSinceDropped ), Color.Blue, 0, 500 );
						break;
					default:
						break;
				}

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

			if ( CanSecondaryAttack() )
			{
				using ( LagCompensation() )
				{
					AttackSecondary();
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

		public virtual bool CanSecondaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Attack2 ) )
			{
				HoldingSecondary = false;
				TimeSinceHoldingSecondary = 0f;
				return false;
			}

			return true;
		}

		async Task DoThrowDelayed()
		{
			await Task.DelaySeconds( 0.2175f );

			ThrowRoll();
		}

		public void ThrowRoll()
		{
			if ( Owner is not FortwarsPlayer player )
				return;

			CantPickup = true;

			if ( IsServer )
				Game.Instance.PlayerDropFlag( player );

			Vector3 throwdir = player.EyeRotation.Forward + (Vector3.Up / 3f);

			player.Inventory.Drop( this );
			player.Inventory.SetActiveSlot( 0, true );

			Velocity = throwdir * 300f * (1f + Math.Clamp( TimeSinceHoldingSecondary / 2f, 0, 2f ));
		}

		[Net] bool HoldingSecondary { get; set; }
		[Net] TimeSince TimeSinceHoldingSecondary { get; set; }

		public virtual void AttackSecondary()
		{
			HoldingSecondary = true;
			ViewModelEntity?.SetAnimParameter( "pull", HoldingSecondary );
			ViewModelEntity?.SetAnimParameter( "pullamount", Math.Clamp( TimeSinceHoldingSecondary / 4f, 0, 1f ) );
		}

		public virtual void AttackPrimary()
		{
			var player = Owner as FortwarsPlayer;
			player.SetAnimParameter( "b_attack", true );
			foreach ( var tr in TraceHit( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 128f ) )
			{
				ViewModelEntity?.SetAnimParameter( "hit", tr.Hit );

				if ( !tr.Hit )
				{
					MissEffects();
					continue;
				}

				HitEffects();

				tr.Entity.TakeDamage( DamageInfo.FromBullet( tr.EndPosition, -tr.Normal * 10f, 10 ) );
			}

			ViewModelEntity?.SetAnimParameter( "fire", true );

			if ( HoldingSecondary )
			{
				_ = DoThrowDelayed();
				ViewModelEntity?.SetAnimParameter( "throw", true );
			}
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

		public override void SimulateAnimator( PawnAnimator anim )
		{
			anim.SetAnimParameter( "holdtype", 4 );
			anim.SetAnimParameter( "aimat_weight", 1.0f );
			anim.SetAnimParameter( "holdtype_handedness", 1 );
			anim.SetAnimParameter( "holdtype_pose_hand", 0.07f );
			anim.SetAnimParameter( "holdtype_attack", 1 );
		}
	}
}
