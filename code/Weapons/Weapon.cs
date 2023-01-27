namespace Fortwars;

[Category( "Weapons" )]
public partial class Weapon : AnimatedEntity
{
	public WeaponViewModel ViewModelEntity { get; protected set; }
	public Player Player => Owner as Player;

	public bool TryRemakeViewModel = false;

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

	public override void Simulate( IClient client )
	{
		SimulateComponents( client );

		// Hacky workaround for when the Asset doesn't propogate
		// fast enough to load the ViewModel successfully.
		if ( Game.IsServer && TryRemakeViewModel )
			CreateViewModel( To.Single( client ) );
	}

	public void OnDeploy( Player player )
	{
		SetParent( player, true );
		Owner = player;

		EnableDrawing = true;

		if ( Game.IsServer )
			CreateViewModel( To.Single( player ) );
	}

	public void OnHolster( Player player )
	{
		EnableDrawing = false;
	}

	[ClientRpc]
	public void CreateViewModel()
	{
		if ( Asset == null )
		{
			SetTryRemake( true );
			return;
		}

		var viewModel = new WeaponViewModel( this );
		viewModel.Model = WeaponAsset.CachedViewModel;
		ViewModelEntity = viewModel;

		SetTryRemake( false );
	}

	[ConCmd.Server]
	public static void SetTryRemake( bool value )
	{
		if ( ConsoleSystem.Caller is not IClient caller )
			return;

		if ( caller.Pawn is not Player player )
			return;

		player.ActiveWeapon.TryRemakeViewModel = value;
	}

	protected override void OnDestroy()
	{
		ViewModelEntity?.Delete();
	}
}
