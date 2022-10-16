// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

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
		var fovTarget = 0f;

		if ( Controller is FortwarsWalkController { IsSprinting: true } )
			fovTarget = 10f;
		if ( Controller is FortwarsWalkController { DuckSlide: DuckSlide { IsActiveSlide: true } } )
			fovTarget = 15f;

		fovOffset = fovOffset.LerpTo( fovTarget, Time.Delta * 20.0f );
		setup.FieldOfView += fovOffset;

		var posOffset = Bobbing.CalcBobbingOffset( ref currentBob, Velocity, this );
		posOffset *= setup.Rotation;
		setup.Position += posOffset;

		lastCameraPos = setup.Position;
	}
}
