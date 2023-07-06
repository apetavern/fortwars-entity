namespace Fortwars;

public partial class Inventory : EntityComponent<Player>, ISingletonComponent
{
	[Net]
	protected IList<Weapon> Weapons { get; set; }

	[Net, Predicted]
	public Weapon ActiveWeapon { get; set; }

	[Net, Predicted]
	public int LastActiveWeaponSlot { get; set; }

	public int GetSlotFromWeapon( Weapon weapon ) => (int)weapon.InventorySlot;
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
			currentWeapon.OnHolster();

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
			currentWeapon.OnHolster();
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
				.Where( wep => wep.InventorySlot == InventorySlot.Primary )
				.FirstOrDefault(),
			1 => Weapons
				.Where( wep => wep.InventorySlot == InventorySlot.Secondary )
				.FirstOrDefault(),
			2 => Weapons
				.Where( wep => wep.InventorySlot == InventorySlot.Equipment )
				.FirstOrDefault(),
			3 => Weapons
				.Where( wep => wep.InventorySlot == InventorySlot.Other )
				.FirstOrDefault(),
			4 => Weapons
				.Where( wep => wep.InventorySlot == InventorySlot.Flag )
				.FirstOrDefault(),
			_ => null
		};
	}

	protected static int GetSlotIndexFromInput( string slot )
	{
		return slot switch
		{
			InputAction.Slot1 => 0,
			InputAction.Slot2 => 1,
			InputAction.Slot3 => 2,
			InputAction.Slot4 => 3,
			InputAction.Slot5 => 4,
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

	protected void TrySlotFromInput( string slot )
	{
		if ( Input.Pressed( slot ) )
		{
			SetWeaponFromSlot( GetSlotIndexFromInput( slot ) );
		}
	}

	public void BuildInput()
	{
		TrySlotFromInput( InputAction.Slot1 );
		TrySlotFromInput( InputAction.Slot2 );
		TrySlotFromInput( InputAction.Slot3 );
		TrySlotFromInput( InputAction.Slot4 );
		TrySlotFromInput( InputAction.Slot5 );

		if ( Input.Pressed( InputAction.Menu ) )
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
		/*		var weaponAsset = WeaponAsset.FromPath( preppedWeaponPath );
				var weapon = WeaponAsset.CreateInstance( weaponAsset );*/

		// player.Inventory.AddWeapon( weapon, true );
	}

	[ConVar.Replicated( "fw_debug_inv" )]
	public static bool Debug { get; set; } = false;
}
