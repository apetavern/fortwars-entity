namespace Fortwars;

public sealed class BogRollItem : Item
{
	public override InventorySlot Slot => InventorySlot.Gamemode;

	public override void OnPickup( Entity carrier )
	{
		// Tell Gamemode we've picked up the toilet roll
	}

	public override void OnDrop()
	{
		// Tell Gamemode we're no longer carrying it
	}
}
