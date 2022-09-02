// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public class FirstPersonCamera : Sandbox.FirstPersonCamera
{
	private float desiredFov;
	private Vector2 totalRecoil; // Get ready for a surprise!

	public override void Build( ref CameraSetup camSetup )
	{
		desiredFov = camSetup.FieldOfView;

		base.Build( ref camSetup );
	}

	public override void Update()
	{
		base.Update();

		if ( Local.Pawn is FortwarsPlayer player
			&& player.ActiveChild is FortwarsWeapon { IsAiming: true } weapon )
		{
			FieldOfView = FieldOfView.LerpTo( weapon.WeaponAsset.AimFovMult * desiredFov, 25 * Time.Delta );
		}
		else
		{
			FieldOfView = FieldOfView.LerpTo( desiredFov, 25 * Time.Delta );
		}

		ZNear = 10;
		ZFar = 20000;
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		const float RecoilTightnessFactor = 2.5f;
		const float RecoilRecoveryScaleFactor = 5f;

		if ( Local.Pawn is not FortwarsPlayer player )
			return;

		if ( player.ActiveChild is not FortwarsWeapon weapon )
			return;

		if ( !weapon.IsValid() )
			return;

		var oldPitch = input.ViewAngles.pitch;
		var oldYaw = input.ViewAngles.yaw;

		totalRecoil -= weapon.Recoil * Time.Delta;

		//
		// Apply recoil - move player's camera in the direction of weapon recoil,
		// tracking how much we apply so that we can recover it later
		//
		input.ViewAngles.pitch -= weapon.Recoil.y * Time.Delta;
		input.ViewAngles.yaw -= weapon.Recoil.x * Time.Delta;

		weapon.Recoil -= weapon.Recoil
			.WithY( ( oldPitch - input.ViewAngles.pitch ) * RecoilTightnessFactor * 1f )
			.WithX( ( oldYaw - input.ViewAngles.yaw ) * RecoilTightnessFactor * 1f );

		//
		// Recover recoil - automatically bring the player's camera back down to its
		// initial offset, based on the total recoil value we tracked
		//
		var delta = totalRecoil;
		totalRecoil = Vector2.Lerp( totalRecoil, 0, RecoilRecoveryScaleFactor * Time.Delta );
		delta -= totalRecoil;

		input.ViewAngles.pitch -= delta.y;
		input.ViewAngles.yaw -= delta.x;
	}
}
