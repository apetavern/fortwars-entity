// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public class FirstPersonCamera : Sandbox.FirstPersonCamera
{
	private float desiredFov;

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
			FieldOfView = FieldOfView.LerpTo( weapon.WeaponAsset.AimFovMult * desiredFov, 10 * Time.Delta );
		}
		else
		{
			FieldOfView = FieldOfView.LerpTo( desiredFov, 10 * Time.Delta );
		}

		ZNear = 1;
		ZFar = 20000;
	}
}
