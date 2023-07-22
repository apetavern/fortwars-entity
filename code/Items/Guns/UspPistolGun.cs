namespace Fortwars;

[Title( "USP Pistol" )]
public sealed partial class UspPistolGun : Gun
{
	protected override Setup Default => new Setup()
	{
		
	};
	
	protected override Model ViewModel => Model.Load( "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl" );
}
