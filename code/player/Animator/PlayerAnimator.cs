namespace Fortwars;

public partial class PlayerAnimator : EntityComponent<Player>, ISingletonComponent
{
	public virtual void Simulate( IClient client )
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
			player.SetAnimParameter( "holdtype", (int)weapon.WeaponAsset.HoldType );
			player.SetAnimParameter( "holdtype_handedness", (int)weapon.WeaponAsset.Handedness );
		}
	}
}
