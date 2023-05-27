namespace Fortwars;

public class JumpMechanic : PlayerControllerMechanic
{
	public static float JumpPower => 280.0f;

	protected override bool ShouldStart()
	{
		if ( GroundEntity.IsValid() )
		{
			if ( !Input.Down( InputAction.Jump ) )
				return false;
		}
		else
		{
			if ( !Input.Pressed( InputAction.Jump ) )
				return false;
		}

		return GroundEntity != null;
	}

	protected override void OnStart()
	{
		var jumpMultiplier = Player.Class.JumpMultiplier * JumpPower;
		var wish = Controller.GetWishInput();

		Velocity += wish * 10f;

		Velocity = Velocity.WithZ( jumpMultiplier );
		Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

		Controller.GetMechanic<WalkMechanic>().ClearGroundEntity();

		IsActive = false;
	}
}
