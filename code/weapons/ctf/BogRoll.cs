// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using static Sandbox.CitizenAnimationHelper;

namespace Fortwars;

[Library( "bogroll", Title = "Bogroll Weapon" )]
public partial class BogRoll : MeleeWeapon
{
	[Net] public Team Team { get; set; }

	[Net] private bool CanPickup { get; set; } = true;
	[Net] private RollReturnZone ReturnZone { get; set; }

	[Net] private bool IsDropped { get; set; }
	[Net] public TimeSince TimeSinceDropped { get; set; }

	public override float PrimaryRate => 2.0f;
	public override string ViewModelPath => "models/items/bogroll/bogroll_v.vmdl";
	private float DropTimer => 15f; // How long can the flag stay dropped for

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/items/bogroll/bogroll_w.vmdl" );
	}

	public override void Simulate( IClient player )
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

			if ( Team == Team.Blue && FortwarsGame.Instance.BlueFlagCarrier != player )
				FortwarsGame.Instance.PlayerPickupEnemyFlagFloor( player );

			if ( Team == Team.Red && FortwarsGame.Instance.BlueFlagCarrier != player )
				FortwarsGame.Instance.PlayerPickupEnemyFlagFloor( player );

			base.OnCarryStart( carrier );
		}
	}

	public override bool CanCarry( Entity carrier ) => CanPickup && ( carrier as FortwarsPlayer ).TeamID != Team;

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		ThrowRoll();

		TimeSinceDropped = 0;
		IsDropped = true;
		ReturnZone = new RollReturnZone();
		ReturnZone?.AttachToRoll( this );

		dropped = true;
		base.ActiveEnd( ent, dropped );
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		IsDropped = false;

		if ( ReturnZone != null && Game.IsServer )
			ReturnZone.Delete();
	}

	protected override void OnDestroy()
	{
		if ( Game.IsServer )
		{
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

		if ( Game.IsServer )
			FortwarsGame.Instance.PlayerDropFlag( player );

		Vector3 throwDir = ( player.EyeRotation.Forward + ( Vector3.Up / 3f ) ).Normal;

		player.Inventory.Drop( this );
		player.Inventory.SetActiveSlot( 0, true );

		Tags.Add( "trigger" );

		Velocity = throwDir * 600f;
	}

	public override void SimulateAnimator( CitizenAnimationHelper animHelper )
	{
		animHelper.HoldType = HoldTypes.HoldItem;
		animHelper.Handedness = Hand.Right;
		// anim.SetAnimParameter( "holdtype_pose_hand", 0.07f );
		// anim.SetAnimParameter( "holdtype_attack", 1 );
	}

	[Event.Tick.Server]
	public void OnServerTick()
	{
		if ( IsDropped && TimeSinceDropped > DropTimer )
		{
			PlaySound( "enemyflagreturned" );//Play sad flag return sounds
			Delete();
			FortwarsGame.Instance.ReturnFlag( Team );
		}
	}

	[Event.Tick.Client]
	public void OnClientTick()
	{
		if ( IsDropped )
		{
			switch ( Team )
			{
				case Team.Invalid:
					break;
				case Team.Red:
					DebugOverlay.Text(
						"" + ( DropTimer - TimeSinceDropped ).CeilToInt(),
						CollisionWorldSpaceCenter + Vector3.Up * 20f,
						Color.Red,
						0,
						500 );
					break;
				case Team.Blue:
					DebugOverlay.Text(
						"" + ( DropTimer - TimeSinceDropped ).CeilToInt(),
						CollisionWorldSpaceCenter + Vector3.Up * 20f,
						Color.Blue,
						0,
						500 );
					break;
				default:
					break;
			}
		}
	}
}
