namespace Fortwars;

public class AirMoveMechanic : PlayerControllerMechanic
{
	public static float Gravity => 800.0f;
	public static float AirControl => 32.0f;
	public static float AirAcceleration => 8.0f;

	protected override void Simulate()
	{
		var ctrl = Controller;
		ctrl.Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;
		ctrl.Velocity += new Vector3( 0, 0, ctrl.BaseVelocity.z ) * Time.Delta;
		ctrl.BaseVelocity = ctrl.BaseVelocity.WithZ( 0 );

		var groundedAtStart = GroundEntity.IsValid();

		if ( groundedAtStart )
			return;

		var wishVel = ctrl.GetWishVelocity( true );
		var wishdir = wishVel.Normal;
		var wishspeed = wishVel.Length;

		ctrl.Accelerate( wishdir, wishspeed, AirControl, AirAcceleration );
		ctrl.Velocity += ctrl.BaseVelocity;
		ctrl.Move();
		ctrl.Velocity -= ctrl.BaseVelocity;
		ctrl.Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;
	}

	protected override bool ShouldStart()
	{
		return true;
	}
}
