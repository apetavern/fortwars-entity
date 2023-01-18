namespace Fortwars;

public partial class Player
{
	public ClassAsset Class { get; set; } = ClassAsset.Default;

	[Net]
	public string SelectedClass { get; set; } = "data/classes/assault.fwclass";

	protected void SetupController()
	{
		Components.Create<PlayerController>();
		Components.RemoveAny<PlayerControllerMechanic>();

		// Apply controller mechanics common to all classes.
		Components.Create<WalkMechanic>();
		Components.Create<AirMoveMechanic>();

		Log.Info( $"Game IsServer: {Game.IsServer} -- {Class}" );

		// Apply controller mechanics specific to classes.
		switch ( Class.ClassName )
		{
			case "Assault":
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
}
