namespace Fortwars;

public class SprintMechanic : PlayerControllerMechanic
{
	public override float WishSpeed => 375f * Player.Class.SpeedMultiplier;

	public override int SortOrder { get; set; } = 25;

	protected override bool ShouldStart()
	{
		return Input.Down( InputButton.Run ) && GroundEntity != null;
	}
}
