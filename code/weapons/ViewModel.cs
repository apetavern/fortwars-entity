using Sandbox;
using System;

namespace Fortwars
{
	public class ViewModel : BaseViewModel
	{
		protected float SwingInfluence => 0.05f;
		protected float ReturnSpeed => 5.0f;
		protected float MaxOffsetLength => 20.0f;
		protected float BobCycleTime => 7.0f;
		protected Vector3 BobDirection => new( 0.0f, 0.5f, 1.0f );

		private Vector3 swingOffset;
		private float lastPitch;
		private float lastYaw;
		private float bobAnim;

		private bool activated;

		private Vector3 ViewmodelOffset => new( -2, 5, 1 );

		private Vector3 ShootOffset { get; set; }
		private FortwarsWeapon Weapon { get; set; }

		public ViewModel( FortwarsWeapon weapon )
		{
			Weapon = weapon;
		}

		public void OnFire()
		{
			using ( Prediction.Off() )
				ShootOffset -= Vector3.Backward * 2;
		}

		Vector3 TargetPos = 0;
		Vector3 FinalPos = 0;
		float TargetFov = 0;
		float FinalFov = 0;
		Rotation TargetRot = Rotation.Identity;
		Rotation FinalRot = Rotation.Identity;

		float LerpSpeed = 10f;

		public override void PostCameraSetup( ref CameraSetup camSetup )
		{
			base.PostCameraSetup( ref camSetup );

			if ( !Local.Pawn.IsValid() )
				return;

			if ( !activated )
			{
				lastPitch = camSetup.Rotation.Pitch();
				lastYaw = camSetup.Rotation.Yaw();

				activated = true;
			}

			Position = camSetup.Position;
			Rotation = camSetup.Rotation;

			FinalRot = Rotation.Lerp( FinalRot, TargetRot, LerpSpeed * Time.Delta );
			Rotation *= FinalRot;

			FinalPos = FinalPos.LerpTo( TargetPos, LerpSpeed * Time.Delta );
			Position += (FinalPos + ViewmodelOffset) * Rotation;

			FinalFov = FinalFov.LerpTo( TargetFov, LerpSpeed * Time.Delta );
			camSetup.ViewModel.FieldOfView = FinalFov;

			TargetPos = 0;
			TargetFov = 65;
			TargetRot = Rotation.Identity;
			LerpSpeed = 10f;

			float bobCycleTime = BobCycleTime;

			if ( DoTucking() )
				return;

			if ( DoSprinting() )
				bobCycleTime *= 2;

			DoBobbing( bobCycleTime );

			DoShootOffset();
		}

		private bool DoSprinting()
		{
			// Todo: this is a bit hacky. We should probably make a custom player controller with an "isSprinting" property/field
			if ( Local.Pawn is Player { Controller: { Velocity: { Length: > 250 } } } player && Input.Down( InputButton.Run ) )
			{
				TargetRot = Rotation.From( 10, 25, 0 );
				TargetPos = Vector3.Backward * 6f;
				return true;
			}

			return false;
		}

		private bool DoBobbing( float bobCycleTime )
		{
			var newPitch = Rotation.Pitch();
			var newYaw = Rotation.Yaw();

			var pitchDelta = Angles.NormalizeAngle( newPitch - lastPitch );
			var yawDelta = Angles.NormalizeAngle( lastYaw - newYaw );

			var playerVelocity = Local.Pawn.Velocity;
			var verticalDelta = playerVelocity.z * Time.Delta;
			var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
			verticalDelta *= (1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y ));
			pitchDelta -= verticalDelta * 1;

			var offset = CalcSwingOffset( pitchDelta, yawDelta );
			offset += CalcBobbingOffset( playerVelocity, bobCycleTime );
			offset -= ShootOffset;
			TargetPos += offset;

			lastPitch = newPitch;
			lastYaw = newYaw;

			return true;
		}

		private bool DoTucking()
		{
			var tuckDist = Weapon.GetTuckDist();
			if ( tuckDist != -1 )
			{
				var t = Math.Min( 1, ((32f - tuckDist) / 32f) + 0.5f );
				TargetRot = Rotation.From( 15, 15, 0 ) * t;

				return true;
			}
			else
			{
				return false;
			}
		}

		private bool DoShootOffset()
		{
			ShootOffset = ShootOffset.LerpTo( Vector3.Zero, 2.5f * Time.Delta );

			return true;
		}

		protected Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
		{
			Vector3 swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

			swingOffset -= swingOffset * ReturnSpeed * Time.Delta;
			swingOffset += (swingVelocity * SwingInfluence);

			if ( swingOffset.Length > MaxOffsetLength )
			{
				swingOffset = swingOffset.Normal * MaxOffsetLength;
			}

			return swingOffset;
		}

		protected Vector3 CalcBobbingOffset( Vector3 velocity, float bobCycleTime )
		{
			var halfPI = MathF.PI * 0.5f;
			var twoPI = MathF.PI * 2.0f;

			if ( Owner.GroundEntity != null )
			{
				bobAnim += Time.Delta * bobCycleTime;
			}
			else
			{
				// In air - return to center
				if ( bobAnim > halfPI + 0.1f )
					bobAnim -= Time.Delta * bobCycleTime * 0.05f;
				else if ( bobAnim < halfPI + 0.1f )
					bobAnim += Time.Delta * bobCycleTime * 0.05f;
				else
					bobAnim = halfPI;
			}

			if ( bobAnim > twoPI )
			{
				bobAnim -= twoPI;
			}

			var speed = new Vector2( velocity.x, velocity.y ).Length;
			speed = speed > 10.0 ? speed : 0.0f;
			var offset = BobDirection * (speed * 0.005f) * MathF.Cos( bobAnim );
			offset = offset.WithZ( -MathF.Abs( offset.z ) );

			return offset;
		}
	}
}
