namespace Fortwars;

public partial class Weapon
{
	[Net]
	private WeaponAsset Asset { get; set; }

	public WeaponAsset WeaponAsset
	{
		get => Asset;
		set
		{
			Asset = value;
			SetupAsset( value );
		}
	}

	protected void SetupAsset( WeaponAsset asset )
	{
		Model = asset.CachedWorldModel;
	}
}
