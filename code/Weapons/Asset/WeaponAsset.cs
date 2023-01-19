namespace Fortwars;

[GameResource( "Weapon Asset", "fwweapon", "A Fortwars Weapon Asset", 
	Icon = "💀", IconBgColor = "#fe71dc", IconFgColor = "black" )]
public partial class WeaponAsset : GameResource
{
	//
	// Meta
	//
	[Category( "Meta" )]
	public string Name { get; set; } = "My weapon";

	[Category( "Meta" ), ResourceType( "vmdl" )]
	public string WorldModel { get; set; }
	internal Model CachedWorldModel;

	[Category( "Meta" ), ResourceType( "vmdl" )]
	public string ViewModel { get; set; }
	internal Model CachedViewModel;

	[Category( "Meta" ), ResourceType( "png" )]
	public string IconPath { get; set; }

	[Category( "Meta" ), Range( 0.1f, 2.0f )]
	public float MovementSpeedMultiplier { get; set; } = 1.0f;

	[Category( "Meta" )]
	public InventorySlots InventorySlot { get; set; } = InventorySlots.Primary;

	//
	// Animation
	//
	[Category( "Animation" )]
	public HoldType HoldType { get; set; } = HoldType.None;

	[Category( "Animation" )]
	public Handedness Handedness { get; set; } = Handedness.Both;


	public static List<WeaponAsset> All { get; set; } = new();

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( !All.Contains( this ) )
			All.Add( this );

		if ( !string.IsNullOrEmpty( WorldModel ) )
			CachedWorldModel = Model.Load( WorldModel );

		if ( !string.IsNullOrEmpty( ViewModel ) )
			CachedViewModel = Model.Load( ViewModel );
	}
}
