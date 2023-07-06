namespace Fortwars;

[Prefab, Category( "Weapons" )]
public partial class Weapon : AnimatedEntity
{
	/// <summary>
	/// The Owner of this Weapon as a Player.
	/// </summary>
	public Player Player => Owner as Player;

	/// <summary>
	/// The Entity holding our ViewModel. This should only exist clientside.
	/// </summary>
	public ViewModel ViewModelEntity { get; set; }
	public virtual ModelEntity EffectEntity => ( ViewModelEntity.IsValid() && IsFirstPersonMode ) ? ViewModelEntity : this;

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
		foreach(var component in WeaponComponents)
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

		ViewModelEntity?.Delete();
	}

	/// <summary>
	/// Handles any logic for deploying this weapon.
	/// </summary>
	/// <param name="player"></param>
	public void OnDeploy( Player player )
	{
		SetParent( player, true );
		Owner = player;

		if ( Game.IsServer )
		{
			CreateViewModel( To.Single( player ) );
		}

		EnableDrawing = true;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( Game.IsClient )
		{
			ViewModelEntity?.Delete();
		}
	}

	[ClientRpc]
	public void CreateViewModel()
	{
		ViewModelEntity = new ViewModel( this )
		{
			Owner = Owner,
			Model = ViewModel,
			EnableViewmodelRendering = true,
		};
	}
}
