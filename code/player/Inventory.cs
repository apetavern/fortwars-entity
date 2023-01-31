namespace Fortwars;

public partial class Inventory : EntityComponent<Player>, ISingletonComponent
{
	[Net]
	protected IList<Weapon> Weapons { get; set; }

	[Net, Predicted]
	public Weapon ActiveWeapon { get; set; }

	[Net, Predicted]
	public int LastActiveWeaponSlot { get; set; }

	public int GetSlotFromWeapon( Weapon weapon ) => Weapons.IndexOf( weapon );
	public int ActiveWeaponSlot => GetSlotFromWeapon( ActiveWeapon );

	public bool AddWeapon( Weapon weapon, bool makeActive = true )
	{
		if ( Weapons.Contains( weapon ) )
			return false;

		Weapons.Add( weapon );

		if ( makeActive )
			SetActiveWeapon( weapon );

		return true;
	}

	public void SetActiveWeapon( Weapon weapon )
	{
		var currentWeapon = ActiveWeapon;
		if ( currentWeapon.IsValid() )
		{
			currentWeapon.OnHolster( Entity );

			if ( currentWeapon.IsValid )
				LastActiveWeaponSlot = ActiveWeaponSlot;

			ActiveWeapon = null;
		}

		ActiveWeapon = weapon;
		weapon?.OnDeploy( Entity );
	}

	public void RemoveActiveWeapon()
	{
		var currentWeapon = ActiveWeapon;
		if ( currentWeapon.IsValid() )
		{
			currentWeapon.OnHolster( Entity );
			Weapons.Remove( currentWeapon );
			ActiveWeapon = null;
		}
	}

	public void RemoveWeapon( Weapon weapon )
	{
		Weapons.Remove( weapon );
	}

	public Weapon GetWeaponFromSlot( int slot )
	{
		return slot switch
		{
			0 => Weapons
				.Where( wep => wep.WeaponAsset.InventorySlot == InventorySlots.Primary )
				.FirstOrDefault(),
			1 => Weapons
				.Where( wep => wep.WeaponAsset.InventorySlot == InventorySlots.Secondary )
				.FirstOrDefault(),
			2 => Weapons
				.Where( wep => wep.WeaponAsset.InventorySlot == InventorySlots.Equipment )
				.FirstOrDefault(),
			3 => Weapons
				.Where( wep => wep.WeaponAsset.InventorySlot == InventorySlots.Other )
				.FirstOrDefault(),
			4 => Weapons
				.Where( wep => wep.WeaponAsset.InventorySlot == InventorySlots.Flag )
				.FirstOrDefault(),
			_ => null
		};
	}

	protected static int GetSlotIndexFromInput( InputButton slot )
	{
		return slot switch
		{
			InputButton.Slot1 => 0,
			InputButton.Slot2 => 1,
			InputButton.Slot3 => 2,
			InputButton.Slot4 => 3,
			InputButton.Slot5 => 4,
			_ => -1
		};
	}

	public void SetWeaponFromSlot( int slot )
	{
		if ( slot == ActiveWeaponSlot )
			return;

		if ( GetWeaponFromSlot( slot ) is Weapon weapon )
		{
			RunGameEventSv( "inv.switchweapon" );
			Entity.ActiveWeaponInput = weapon;
		}
	}

	protected void TrySlotFromInput( InputButton slot )
	{
		if ( Input.Pressed( slot ) )
		{
			Input.SuppressButton( slot );

			SetWeaponFromSlot( GetSlotIndexFromInput( slot ) );
		}
	}

	public void BuildInput()
	{
		TrySlotFromInput( InputButton.Slot1 );
		TrySlotFromInput( InputButton.Slot2 );
		TrySlotFromInput( InputButton.Slot3 );
		TrySlotFromInput( InputButton.Slot4 );
		TrySlotFromInput( InputButton.Slot5 );

		if ( Input.Pressed( InputButton.Menu ) )
			SetWeaponFromSlot( LastActiveWeaponSlot );
	}

	public void Simulate( IClient client )
	{
		if ( Entity.ActiveWeaponInput != null && ActiveWeapon != Entity.ActiveWeaponInput )
		{
			SetActiveWeapon( Entity.ActiveWeaponInput as Weapon );
			Entity.ActiveWeaponInput = null;
		}

		ActiveWeapon?.Simulate( client );

		if ( Debug )
		{
			var lineOffset = 22;
			DebugOverlay.ScreenText( $"ActiveWeapon {ActiveWeapon.WeaponAsset.ResourceName}", lineOffset++ );
			DebugOverlay.ScreenText( $"ActiveWeaponSlot {ActiveWeaponSlot}", lineOffset++ );
			DebugOverlay.ScreenText( $"LastActiveWeaponSlot {LastActiveWeaponSlot}", lineOffset++ );
		}
	}

	public void FrameSimulate( IClient client )
	{
		ActiveWeapon?.FrameSimulate( client );
	}

	public void RunGameEvent( string eventName )
	{
		Entity?.RunGameEvent( eventName );
	}

	[ConCmd.Server]
	public static void RunGameEventSv( string eventName )
	{
		if ( ConsoleSystem.Caller is not IClient caller )
			return;

		if ( caller.Pawn is not Player player )
			return;

		player?.RunGameEvent( eventName );
	}

	[ConCmd.Admin(
		Name = "fw_set_weapon",
		Help = "Set the caller's weapon by asset name (e.g. `fw_set_weapon ksr1`)." )]
	public static void SetWeapon( string weaponPath )
	{
		if ( ConsoleSystem.Caller is not IClient caller )
			return;

		if ( caller.Pawn is not Player player )
			return;

		var preppedWeaponPath = $"data/weapons/{weaponPath}.fwweapon";
		var weaponAsset = WeaponAsset.FromPath( preppedWeaponPath );
		var weapon = WeaponAsset.CreateInstance( weaponAsset );

		player.Inventory.AddWeapon( weapon, true );
	}

	[ConVar.Replicated( "fw_debug_inv" )]
	public static bool Debug { get; set; } = false;
}
