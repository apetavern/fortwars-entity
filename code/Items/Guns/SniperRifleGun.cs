namespace Fortwars;

[Title( "Sniper" )]
public sealed partial class SniperRifleGun : Gun
{
	public override InventorySlot Slot => InventorySlot.Other;

	protected override Setup Default => new Setup()
	{
		RateOfFire = 12,
	};
	
	protected override Model ViewModel => Model.Load( "models/weapons/aiax50/aiax50_v.vmdl" );
}
