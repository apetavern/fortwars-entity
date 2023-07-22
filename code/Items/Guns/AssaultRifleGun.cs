namespace Fortwars;

public sealed partial class AssaultRifleGun : Gun
{
	protected override Setup Default => new Setup()
	{
		MoveSpeedMultiplier = 1,
	};

	protected override Model ViewModel => Model.Load( "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl" );
}
