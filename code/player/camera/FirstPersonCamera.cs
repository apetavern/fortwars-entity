// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public class FirstPersonCamera : Sandbox.FirstPersonCamera
{
	private float desiredFov;
	private Vector2 totalRecoil; // Get ready for a surprise!

	private float roll;
	private float rollMul = 0.0f;

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

		const float RecoilTightnessFactor = 20.0f;
		const float RecoilRecoveryScaleFactor = 5f;
		const float RecoilScale = 8.0f;

		if ( Local.Pawn is not FortwarsPlayer player )
			return;

		if ( player.ActiveChild is not FortwarsWeapon weapon )
			return;

		if ( !weapon.IsValid() )
			return;

		float deltaTime = Time.Delta;
		deltaTime = 1 / 120f; // Don't know why, but using actual deltatime for this fucks everything

		var recoil = weapon.Recoil * RecoilScale;

		var oldPitch = input.ViewAngles.pitch;
		var oldYaw = input.ViewAngles.yaw;

		totalRecoil -= recoil * deltaTime;

		//
		// Apply recoil - move player's camera in the direction of weapon recoil,
		// tracking how much we apply so that we can recover it later
		//
		input.ViewAngles.pitch -= recoil.y * deltaTime;
		input.ViewAngles.yaw -= recoil.x * deltaTime;

		weapon.Recoil -= weapon.Recoil
			.WithY( ( oldPitch - input.ViewAngles.pitch ) * RecoilTightnessFactor * 1f )
			.WithX( ( oldYaw - input.ViewAngles.yaw ) * RecoilTightnessFactor * 1f );

		//
		// Recover recoil - automatically bring the player's camera back down to its
		// initial offset, based on the total recoil value we tracked
		//
		var delta = totalRecoil;
		totalRecoil = Vector2.Lerp( totalRecoil, 0, RecoilRecoveryScaleFactor * deltaTime );
		delta -= totalRecoil;

		input.ViewAngles.pitch -= delta.y;
		input.ViewAngles.yaw -= delta.x;

		rollMul += weapon.Recoil.Length / 10f;
		rollMul = rollMul.LerpTo( 0.0f, 10f * Time.Delta );
		rollMul = rollMul.Clamp( 0, 1 );

		//
		// Roll:
		// This applies some perlin noise multiplied by a bounce value to help things
		// feel a lot meatier while still feeling natural
		//
		var rollCoords = weapon.Recoil + ( Time.Now * 100 );
		var targetRoll = Noise.Perlin( rollCoords.x, rollCoords.y );

		targetRoll = ( -1.0f ).LerpTo( 1.0f, targetRoll );
		targetRoll *= weapon.WeaponAsset.KickbackStrength * Easing.BounceInOut( rollMul );

		roll = roll.LerpTo( targetRoll, 30f * Time.Delta );

		input.ViewAngles = new Angles(
			input.ViewAngles.pitch.Clamp( -89, 89 ),
			input.ViewAngles.yaw.NormalizeDegrees(),
			roll
		); ;
	}
}
