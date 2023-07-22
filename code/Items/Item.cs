namespace Fortwars;

public abstract class Item : AnimatedEntity
{
	protected Item()
	{
		Transmit = TransmitType.Always;
	}

	public InventorySlot Slot { get; set; } = InventorySlot.Primary;
	public HoldType HoldType { get; set; } = HoldType.Rifle;

	public override void Spawn()
	{
		base.Spawn();
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public virtual void OnDeploy()
	{
		EnableDrawing = true;
	}

	public virtual void OnHolster( bool dropping )
	{
		EnableDrawing = false;
	}

	public virtual void OnPickup( Entity carrier )
	{
		SetParent( carrier, true );
		EnableDrawing = false;
	}

	public virtual void OnDrop()
	{
		Parent = null;
		EnableDrawing = true;
	}
}
