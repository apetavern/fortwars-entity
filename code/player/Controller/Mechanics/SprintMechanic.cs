namespace Fortwars;

public class SprintMechanic : PlayerControllerMechanic
{
	public override float WishSpeed { get; protected set; } = 375f;

	public override int SortOrder { get; set; } = 25;

	protected override bool ShouldStart()
	{
		return Input.Down( InputButton.Run ) && GroundEntity != null;
	}
}
