namespace Fortwars;

public partial class Weapon : AnimatedEntity
{
	public WeaponViewModel ViewModelEntity { get; protected set; }
	public Player Player => Owner as Player;

	public override void Spawn()
	{
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableDrawing = false;
	}

	public override void Simulate( IClient client )
	{
		SimulateComponents( client );
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
		var viewModel = new WeaponViewModel( this );
		viewModel.Model = WeaponAsset.CachedViewModel;
		ViewModelEntity = viewModel;
	}

	protected override void OnDestroy()
	{
		ViewModelEntity?.Delete();
	}
}
