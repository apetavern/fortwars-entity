using System.Runtime.CompilerServices;

namespace Fortwars;

public struct AssetPtr<T> where T : Resource
{
	public AssetPtr( string ident )
	{
		Ident = ident.FastHash();
	}

	public int Ident;

	public T Resolved
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => null; // sbox suxs... doesn't support this yet
	}

	public static implicit operator AssetPtr<T>( string item ) => new AssetPtr<T>( item );
	public static implicit operator T( AssetPtr<T> asset ) => asset.Resolved;
}

public abstract class Item : AnimatedEntity
{
	public virtual void OnDeploy() { }
	public virtual void OnHolster( bool dropping ) { }

	public virtual void OnPickup( Entity carrier )
	{
		SetParent(carrier, true);
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
		ViewModel = "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl",
	};
}

[Category( "Gun" ), Icon( "gavel" )]
public abstract partial class Gun : Item
{
	public struct Setup
	{
		public float MoveSpeedMultiplier;
		public InventorySlot Slot;
		public HoldType HoldType;

		public AssetPtr<Model> ViewModel;
		public AssetPtr<ParticleSystem> TracerParticle;
		public AssetPtr<SoundEvent> ShootSound;
	}

	public Setup Data => n_Data;
	[Net] private Setup n_Data { get; set; }

	protected abstract Setup Default { get; }

	protected Gun()
	{
		Transmit = TransmitType.Always;
	}

	public override void Spawn()
	{
		base.Spawn();
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
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

	public Weapon()
	{
		Transmit = TransmitType.Always;
	}

	public override void Spawn()
	{
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableDrawing = false;
	}

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

	/// <summary>
	/// Handles any logic for holstering this weapon.
	/// </summary>
	public void OnHolster()
	{
		EnableDrawing = false;
	}

	/// <summary>
	/// Handles any logic for deploying this weapon.
	/// </summary>
	/// <param name="player"></param>
	public void OnDeploy( Player player )
	{
		SetParent( player, true );
		Owner = player;
		EnableDrawing = true;
	}
}
