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

public sealed partial class AssaultRifleGun : Gun
{
	protected override Setup Default => new Setup()
	{
		MoveSpeedMultiplier = 1,
	};

	protected override Model ViewModel => Model.Load( "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl" );
}

[Category( "Gun" ), Icon( "gavel" )]
public abstract partial class Gun : Item
{
	public struct Setup
	{
		public float MoveSpeedMultiplier;
		public HoldType HoldType;
	}

	public Setup Data => n_Data;
	[Net] private Setup n_Data { get; set; }

	protected abstract Setup Default { get; }

	protected abstract Model ViewModel { get; }

	protected Gun()
	{
		Transmit = TransmitType.Always;
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
	}
}

[Prefab, Category( "Weapons" )]
public partial class Weapon : AnimatedEntity
{
	/// <summary>
	/// The Owner of this Weapon as a Player.
	/// </summary>
	public Player Player => Owner as Player;

	public IEnumerable<WeaponComponent> WeaponComponents => Components.GetAll<WeaponComponent>( includeDisabled: true );


	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		foreach ( var component in WeaponComponents )
		{
			if ( !component.Enabled )
				continue;

			component.Simulate( cl, Player );
		}
	}
}
