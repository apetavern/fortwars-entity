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

	
}
