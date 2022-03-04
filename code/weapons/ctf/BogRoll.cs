using Sandbox;

namespace Fortwars
{
	[Library( "bogroll", Title = "Bogroll Weapon" )]
	public partial class BogRoll : MeleeWeapon
	{
		[Net] public Team Team { get; set; }
		[Net] public TimeSince TimeSinceDropped { get; set; }
		[Net] bool CanPickup { get; set; } = true;
		[Net] RollReturnZone ReturnZone { get; set; }

		public override float PrimaryRate => 2.0f;
		public override string ViewModelPath => "models/items/bogroll/bogroll_v.vmdl";

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/items/bogroll/bogroll_w.vmdl" );
		}

		public override void Simulate( Client player )
		{
			base.Simulate( player );

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
		}

		public override void OnCarryStart( Entity carrier )
		{
			if ( carrier is FortwarsPlayer player )
			{
				player.Inventory.SetActive( this );

				if ( Team == Team.Blue && Game.Instance.BlueFlagCarrier != player )
					Game.Instance.PlayerPickupEnemyFlagFloor( player );

				if ( Team == Team.Red && Game.Instance.BlueFlagCarrier != player )
					Game.Instance.PlayerPickupEnemyFlagFloor( player );

				base.OnCarryStart( carrier );
			}
		}

		public override bool CanCarry( Entity carrier ) => CanPickup && (carrier as FortwarsPlayer).TeamID != Team;

		public override void ActiveEnd( Entity ent, bool dropped )
		{
			ThrowRoll();

			dropped = true;
			base.ActiveEnd( ent, dropped );
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

		public override void EndTouch( Entity other )
		{
			CanPickup = true;
			base.EndTouch( other );
		}

		public void ThrowRoll()
		{
			if ( Owner is not FortwarsPlayer player )
				return;

			CanPickup = false;

			if ( IsServer )
				Game.Instance.PlayerDropFlag( player );

			Vector3 throwDir = (player.EyeRotation.Forward + (Vector3.Up / 3f)).Normal;

			player.Inventory.Drop( this );
			player.Inventory.SetActiveSlot( 0, true );

			Velocity = throwDir * 600f;
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			anim.SetAnimParameter( "holdtype", 4 );
			anim.SetAnimParameter( "holdtype_handedness", 1 );
			anim.SetAnimParameter( "holdtype_pose_hand", 0.07f );
			anim.SetAnimParameter( "holdtype_attack", 1 );
		}
	}
}
