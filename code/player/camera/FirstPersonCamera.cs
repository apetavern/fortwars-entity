using Sandbox;

namespace Fortwars
{
	public class FirstPersonCamera : Sandbox.FirstPersonCamera
	{
		[ClientVar( "fw_fov_desired", Help = "Desired player field of view", Max = 110f, Min = 80f )]
		public static float FovDesired { get; set; } = 90f;

		public override void Update()
		{
			base.Update();

			if ( Local.Pawn.ActiveChild is FortwarsWeapon { IsAiming: true } weapon )
				FieldOfView = FieldOfView.LerpTo( weapon.WeaponAsset.AimFovMult * FovDesired, 10 * Time.Delta );
			else
				FieldOfView = FieldOfView.LerpTo( FovDesired, 10 * Time.Delta );
		}
	}
}
