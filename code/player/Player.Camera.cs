// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System;

namespace Fortwars;

partial class FortwarsPlayer
{
	private Vector3 lastCameraPos = Vector3.Zero;
	private Rotation lastCameraRot = Rotation.Identity;

	public override void PostCameraSetup( ref CameraSetup setup )
	{
		base.PostCameraSetup( ref setup );

		if ( lastCameraRot == Rotation.Identity )
			lastCameraRot = setup.Rotation;

		var angleDiff = Rotation.Difference( lastCameraRot, setup.Rotation );
		var angleDiffDegrees = angleDiff.Angle();
		var allowance = 20.0f;

		if ( angleDiffDegrees > allowance )
		{
			// We could have a function that clamps a rotation to within x degrees of another rotation?
			lastCameraRot = Rotation.Lerp( lastCameraRot, setup.Rotation, 1.0f - ( allowance / angleDiffDegrees ) );
		}

		if ( setup.Viewer != null )
		{
			AddCameraEffects( ref setup );
		}
	}

	float currentBob = 0;
	float fovOffset = 0;

	private void AddCameraEffects( ref CameraSetup setup )
	{
		var speed = Velocity.Length.LerpInverse( 200, 400 );
		var forwardspeed = Velocity.Normal.Dot( setup.Rotation.Forward );

		fovOffset = fovOffset.LerpTo( speed * 40f * MathF.Abs( forwardspeed ), Time.Delta * 2.0f );
		setup.FieldOfView += fovOffset;

		var posOffset = Bobbing.CalcBobbingOffset( ref currentBob, Velocity, this );
		posOffset *= setup.Rotation;

		setup.Position += posOffset;

		lastCameraPos = setup.Position;
	}
}
