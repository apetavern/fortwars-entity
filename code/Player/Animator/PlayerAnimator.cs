namespace Fortwars;

public sealed class PlayerAnimator : EntityComponent<Player>, ISingletonComponent
{
	public void Simulate( IClient client )
	{
		var player = Entity;
		var ctrl = player.Controller;
		var helper = new CitizenAnimationHelper( player );

		if ( ctrl is null )
			return;

		helper.WithWishVelocity( ctrl.GetWishVelocity() );
		helper.WithVelocity( ctrl.Velocity );
		helper.WithLookAt( player.EyePosition + player.EyeRotation.Forward * 100f, 1.0f, 1.0f, 0.5f );
		helper.AimAngle = player.EyeRotation;
		helper.IsGrounded = ctrl.GroundEntity != null;

		var weapon = player.ActiveWeapon;
		if ( weapon.IsValid() )
		{
			player.SetAnimParameter( "holdtype", (int)weapon.HoldType );
			// player.SetAnimParameter( "holdtype_handedness", (int)weapon.Handedness );

			/*
			foreach ( var component in weapon.WeaponComponents )
			{
				component.SimulateAnimator( player );
			}
			*/
		}
		else
		{
			player.SetAnimParameter( "holdtype", (int)HoldType.None );
		}
	}
}
