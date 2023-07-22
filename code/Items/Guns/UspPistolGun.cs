namespace Fortwars;

[Title( "USP Pistol" )]
public sealed partial class UspPistolGun : Gun
{
	public override InventorySlot Slot => InventorySlot.Secondary;
	
	protected override Setup Default => new Setup()
	{
		
	};
	
	protected override Model ViewModel => Model.Load( "models/weapons/hkusp/hkusp_v.vmdl" );
}
