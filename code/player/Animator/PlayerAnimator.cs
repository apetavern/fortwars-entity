using static Sandbox.Event;

namespace Fortwars;

public partial class PlayerAnimator : EntityComponent<Player>, ISingletonComponent
{
	public virtual void Simulate( IClient client )
	{
		var player = Entity;
		var ctrl = player.Controller;
		var helper = new CitizenAnimationHelper( player );

		helper.WithWishVelocity( ctrl.GetWishVelocity() );
		helper.WithVelocity( ctrl.Velocity );
		helper.WithLookAt( player.EyePosition + player.EyeRotation.Forward * 100f, 1.0f, 1.0f, 0.5f );
		helper.AimAngle = player.EyeRotation;
		helper.IsGrounded = ctrl.GroundEntity != null;

	}
}
