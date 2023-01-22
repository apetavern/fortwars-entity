namespace Fortwars;

public partial class Player
{
	[Net]
	public ClassAsset Class { get; set; } = ClassAsset.Default;

	[Net]
	public string SelectedClass { get; set; } = "data/classes/assault.fwclass";

	[Net]
	public string SelectedPrimaryWeapon { get; set; } = "data/weapons/ksr1.fwweapon";

	[Net]
	public string SelectedSecondaryWeapon { get; set; } = "data/weapons/trj.fwweapon";

	protected void SetupController()
	{
		Components.Create<PlayerController>();
		Components.RemoveAny<PlayerControllerMechanic>();

		// Apply controller mechanics common to all classes.
		Components.Create<WalkMechanic>();
		Components.Create<AirMoveMechanic>();
		Components.Create<JumpMechanic>();
		Components.Create<SprintMechanic>();

		// Create or remove controller mechanics specific to classes.
		switch ( Class.ClassName )
		{
			case "Assault":
				Components.RemoveAny<SprintMechanic>();
				break;
			case "Engineer":
				break;
			case "Flagrunner":
				break;
			case "Medic":
				break;
			case "Support":
				break;
			default:
				break;
		}
	}

	protected void SetupInventory()
	{
		Components.RemoveAny<Inventory>();
		Components.Create<Inventory>();

		GamemodeSystem.Instance?.PrepareLoadout( this, Inventory );
	}

	[ConCmd.Admin( "fw_set_class" )]
	public static void SetClass( string classname )
	{
		if ( ConsoleSystem.Caller is not IClient caller )
			return;

		if ( caller.Pawn is not Player player )
			return;

		var preppedClassString = $"data/classes/{classname}.fwclass";
		player.Class = ClassAsset.FromPath( preppedClassString );
		player.Respawn();
	}
}
