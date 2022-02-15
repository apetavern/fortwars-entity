using Sandbox;
using System;

namespace Fortwars
{
	public class ViewModel : BaseViewModel
	{
		[ClientVar( "fw_viewmodel_fov", Help = "Viewmodel field of view", Min = 50f, Max = 90f )]
		public static float ViewmodelFov { get; set; } = 60f;

		protected float SwingInfluence => 0.05f;
		protected float ReturnSpeed => 5.0f;
		protected float MaxOffsetLength => 20.0f;
		protected float BobCycleTime => 7.0f;
		protected Vector3 BobDirection => new( 0.0f, 0.5f, 1.0f );

		private Vector3 TargetPos = 0;
		private Vector3 FinalPos = 0;
		private float TargetFov = 0;
		private float FinalFov = 0;
		private Rotation TargetRot = Rotation.Identity;
		private Rotation FinalRot = Rotation.Identity;

		private float LerpSpeed = 10f;

		private Vector3 swingOffset;
		private float lastPitch;
		private float lastYaw;
		private float bobAnim;

		private bool activated;

		private Vector3 ShootOffset { get; set; }
		private FortwarsWeapon Weapon { get; set; }

		public ViewModel( FortwarsWeapon weapon )
		{
			Weapon = weapon;
		}

		public ViewModel() { }

		public void OnFire( bool ads )
		{
			using ( Prediction.Off() )
			{
				float strength = 16f;
				ShootOffset += Vector3.Backward * strength;
			}
		}

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
			Position += FinalPos * Rotation;

			FinalFov = FinalFov.LerpTo( TargetFov, LerpSpeed * Time.Delta );
			camSetup.ViewModel.FieldOfView = FinalFov;

			TargetPos = 0;
			TargetFov = ViewmodelFov;
			TargetRot = Rotation.Identity;
			LerpSpeed = 10f;

			float bobCycleTime = BobCycleTime;

			if ( Weapon.IsValid() && DoTucking() )
				return;

			DoShootOffset();

			if ( DoADS() )
			{
				DoBobbing( bobCycleTime );
				return;
			}

			DoDucking();

			if ( DoSprinting() )
				bobCycleTime *= 2;

			if ( DoSliding() )
				return;

			DoBobbing( bobCycleTime );
		}

		private bool DoADS()
		{
			if ( Weapon != null && Weapon.IsAiming )
			{
				TargetPos = Weapon.WeaponAsset.AimPosition;
				TargetRot = Weapon.WeaponAsset.AimRotation.ToRotation();
				TargetFov = Weapon.WeaponAsset.AimFovMult * ViewmodelFov;

				return true;
			}

			return false;
		}

		private bool DoSprinting()
		{
			if ( Local.Pawn is Player { Controller: FortwarsWalkController { IsSprinting: true } } player )
			{
				TargetRot = Rotation.From( 15, 5, 0 );
				TargetPos = Vector3.Backward * 6f;
				return true;
			}

			return false;
		}

		private bool DoSliding()
		{
			if ( Local.Pawn is Player { Controller: FortwarsWalkController { DuckSlide: { IsActive: true, IsActiveSlide: true } } } player )
			{
				TargetRot = Rotation.From( 0, 0, -35 );
				TargetPos = Vector3.Down * 16f + Vector3.Left * 16f;
				return true;
			}

			return false;
		}

		private bool DoDucking()
		{
			if ( Local.Pawn is Player { Controller: FortwarsWalkController { DuckSlide: { IsActive: true, IsActiveSlide: false } } } player )
			{
				TargetRot = Rotation.From( 5, 0, -15 );
				TargetPos = Vector3.Zero;
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

			float aimingMultiplier = 1.0f;

			if ( Weapon != null && Weapon.IsAiming )
				aimingMultiplier = 0.1f;

			var offset = CalcSwingOffset( pitchDelta, yawDelta );
			offset += CalcBobbingOffset( playerVelocity, bobCycleTime );
			offset += ShootOffset;

			if ( Owner.GroundEntity == null )
			{
				offset += new Vector3( 0, 0, -2.5f );
				newPitch -= 2.5f;
			}

			offset *= aimingMultiplier;

			Vector2 maskOffset = new Vector2( offset.y, offset.z ) * 0.1f * (10 * offset.x + 1f);
			SceneObject.SetValue( "maskOffset", new Vector2( maskOffset.x, maskOffset.y ) );

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
			ShootOffset = ShootOffset.LerpTo( Vector3.Zero, 20f * Time.Delta );

			return true;
		}

		public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
		{
			if ( name.Contains( "shake" ) )
			{
				_ = new Sandbox.ScreenShake.Perlin( 1f, 1f, floatData );
			}
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
