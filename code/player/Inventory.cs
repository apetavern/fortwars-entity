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

	public void SetActiveWeapon ( Weapon weapon )
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
}
