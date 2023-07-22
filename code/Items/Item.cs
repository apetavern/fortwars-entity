namespace Fortwars;

public abstract partial class Item : AnimatedEntity
{
	protected Item()
	{
		Transmit = TransmitType.Always;
	}

	public virtual InventorySlot Slot => InventorySlot.Primary;
	public virtual HoldType HoldType => HoldType.Rifle;

	public override void Spawn()
	{
		base.Spawn();
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public virtual void OnDeploy()
	{
		Log.Info( "Deploying Item" );

		EnableDrawing = true;
		CreateViewModel();
	}

	public virtual void OnHolster( bool dropping )
	{
		Log.Info( "Holstering Item" );

		EnableDrawing = false;
		DestroyViewModel();
	}

	public virtual void OnPickup( Entity carrier )
	{
		Log.Info( "Picking up Item" );

		SetParent( carrier, true );
		EnableDrawing = false;
	}

	public virtual void OnDrop()
	{
		Log.Info( "Dropping Item" );

		Parent = null;
		EnableDrawing = true;
	}

	protected virtual Model ViewModel => Model.Load( "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl" );

	private ViewModel viewModelInstance;

	[ClientRpc]
	private void CreateViewModel()
	{
		Game.AssertClient();
		viewModelInstance = new ViewModel( this )
		{
			Model = ViewModel,
			Owner = this
		};
	}

	[ClientRpc]
	private void DestroyViewModel()
	{
		viewModelInstance?.Delete();
		viewModelInstance = null;
	}
}
