using Sandbox;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Fortwars
{
	partial class FortwarsPlayer
	{
		RealTimeSince timeSinceUpdatedFramerate;

		Rotation lastCameraRot = Rotation.Identity;

		public override void PostCameraSetup( Camera camera )
		{
			base.PostCameraSetup( camera );

			if ( lastCameraRot == Rotation.Identity )
				lastCameraRot = Camera.Rot;

			var angleDiff = Rotation.Difference( lastCameraRot, Camera.Rot );
			var angleDiffDegrees = angleDiff.Angle();
			var allowance = 20.0f;

			if ( angleDiffDegrees > allowance )
			{
				// We could have a function that clamps a rotation to within x degrees of another rotation?
				lastCameraRot = Rotation.Lerp( lastCameraRot, Camera.Rot, 1.0f - (allowance / angleDiffDegrees) );
			}
			else
			{
				//lastCameraRot = Rotation.Lerp( lastCameraRot, Camera.Rot, Time.Delta * 0.2f * angleDiffDegrees );
			}

			// uncomment for lazy cam
			//camera.Rot = lastCameraRot;

			if ( camera is FirstPersonCamera )
			{
				AddCameraEffects( camera );
			}

			if ( timeSinceUpdatedFramerate > 1 )
			{
				timeSinceUpdatedFramerate = 0;
				//UpdateFps( (int) (1.0f / Time.Delta) );
			}
		}

		float walkBob = 0;
		float lean = 0;
		float fov = 0;

		private void AddCameraEffects( Camera camera )
		{
			var speed = Velocity.Length.LerpInverse( 0, 320 );
			var forwardspeed = Velocity.Normal.Dot( camera.Rot.Forward );

			var left = camera.Rot.Left;
			var up = camera.Rot.Up;

			if ( GroundEntity != null )
			{
				walkBob += Time.Delta * 25.0f * speed;
			}

			camera.Pos += up * MathF.Sin( walkBob ) * speed * 2;
			camera.Pos += left * MathF.Sin( walkBob * 0.6f ) * speed * 1;

			// Camera lean
			lean = lean.LerpTo( Velocity.Dot( camera.Rot.Right ) * 0.03f, Time.Delta * 15.0f );

			var appliedLean = lean;
			appliedLean += MathF.Sin( walkBob ) * speed * 0.2f;
			camera.Rot *= Rotation.From( 0, 0, appliedLean );

			speed = (speed - 0.7f).Clamp( 0, 1 ) * 3.0f;

			fov = fov.LerpTo( speed * 20 * MathF.Abs( forwardspeed ), Time.Delta * 2.0f );

			camera.FieldOfView += fov;

			//	var tx = new Sandbox.UI.PanelTransform();
			//	tx.AddRotation( 0, 0, lean * -0.1f );

			//	Hud.CurrentPanel.Style.Transform = tx;
			//	Hud.CurrentPanel.Style.Dirty();

		}
	}
}
