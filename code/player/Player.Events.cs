namespace Fortwars;

public partial class Player
{
	public void RunGameEvent( string eventName )
	{
		eventName = eventName.ToLowerInvariant();

		Inventory.ActiveWeapon?.Components.GetAll<WeaponComponent>()
			.ToList()
			.ForEach( x => x.OnGameEvent( eventName ) );
	}
}
