namespace Fortwars;

[Title( "Assault Rifle" )]
public sealed partial class AssaultRifleGun : Gun
{
	protected override Setup Default => new Setup()
	{
		Automatic = true,
		RateOfFire = 550,
		MoveSpeedMultiplier = 1,
	};

	protected override Model ViewModel => Model.Load( "models/weapons/ksr1/ksr1_v.vmdl" );
}
