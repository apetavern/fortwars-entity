// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System;

namespace Fortwars;

public class ViewModel : BaseViewModel
{
	[ClientVar( "fw_viewmodel_fov", Help = "Viewmodel field of view", Min = 50f, Max = 90f )]
	public static float ViewmodelFov { get; set; } = 60f;

	protected float SwingInfluence => 0.05f;
	protected float ReturnSpeed => 5.0f;
	protected float MaxOffsetLength => 20.0f;
	protected Vector3 BobDirection => new( 0.0f, 0.5f, 1.0f );

	private Vector3 TargetPos = 0;
	private Vector3 FinalPos = 0;
	private float TargetFov = 60f;
	private float FinalFov = 60f;
	private Rotation TargetRot = Rotation.Identity;
	private Rotation FinalRot = Rotation.Identity;

	private float LerpSpeed = 10f;

	private Vector3 swingOffset;
	private float lastPitch;
	private float lastYaw;
	private float bobAnim;

	private bool activated;

	private Vector3 ShootOffset { get; set; }
	private Rotation ShootRotation { get; set; }
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
			ShootRotation *= Rotation.FromPitch( -strength );

			if ( !ads )
				SetAnimParameter( "fire", true );
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

		if ( Weapon.IsValid() && DoTucking() )
			return;

		DoShootOffset();

		if ( DoADS() )
		{
			DoBobbing();
			return;
		}

		DoDucking();
		DoSprinting();

		if ( DoSliding() )
			return;

		DoBobbing();
	}

	private bool DoADS()
	{
		if ( Weapon != null && Weapon.IsAiming )
		{
			TargetPos = Weapon.WeaponAsset.AimPosition;
			TargetRot = Weapon.WeaponAsset.AimRotation.ToRotation();
			// TargetFov = Weapon.WeaponAsset.AimFovMult * ViewmodelFov;
			LerpSpeed = 20f;
			return true;
		}

		return false;
	}

	private bool DoSprinting()
	{
		if ( Local.Pawn is Player { Controller: FortwarsWalkController { IsSprinting: true } } player )
		{
			TargetRot = Weapon.WeaponAsset.SprintAnimationType switch
			{
				WeaponAsset.SprintAnimationTypes.Default => Rotation.From( 15, 45, 0 ),
				WeaponAsset.SprintAnimationTypes.Pistol => Rotation.From( -45, 0, 0 ),
				_ => throw new NotImplementedException(),
			};

			TargetPos = Weapon.WeaponAsset.SprintAnimationType switch
			{
				WeaponAsset.SprintAnimationTypes.Default => Vector3.Backward * 32f + Vector3.Right * 16f,
				WeaponAsset.SprintAnimationTypes.Pistol => Vector3.Down * 32f + Vector3.Backward * 32f,
				_ => throw new NotImplementedException(),
			};

			LerpSpeed = 5f;
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

	private bool DoBobbing()
	{
		var newPitch = Rotation.Pitch();
		var newYaw = Rotation.Yaw();

		var pitchDelta = Angles.NormalizeAngle( newPitch - lastPitch );
		var yawDelta = Angles.NormalizeAngle( lastYaw - newYaw );

		var playerVelocity = Local.Pawn.Velocity;
		var verticalDelta = playerVelocity.z * Time.Delta;
		var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
		verticalDelta *= 1.0f - MathF.Abs( viewDown.Cross( Vector3.Down ).y );
		pitchDelta -= verticalDelta * 1;

		var offset = CalcSwingOffset( pitchDelta, yawDelta );
		offset += CalcBobbingOffset( playerVelocity );
		if ( Owner.GroundEntity == null )
		{
			offset += new Vector3( 0, 0, -2.5f );
			newPitch -= 2.5f;
		}

		offset *= ( Weapon?.IsAiming ?? false ) ? 0.1f : 1.0f;

		float offsetMultiplier = 1.0f;
		float rotationMultiplier = 1.0f;

		if ( Weapon != null )
		{
			rotationMultiplier = Weapon.WeaponAsset.ProceduralViewmodelStrength;
			offsetMultiplier = Weapon.WeaponAsset.ProceduralViewmodelStrength;

			if ( Weapon.IsAiming )
			{
				rotationMultiplier = Weapon.WeaponAsset.AimedProceduralViewmodelStrength;
				offsetMultiplier = Weapon.WeaponAsset.AimedProceduralViewmodelStrength;
			}
		}
		offset += ShootOffset * offsetMultiplier;
		var rotationOffset = ShootRotation * rotationMultiplier;

		TargetPos += offset;
		TargetRot *= rotationOffset;

		lastPitch = newPitch;
		lastYaw = newYaw;

		Vector2 maskOffset = new Vector2( offset.y, offset.z ) * 0.1f * ( 10 * offset.x + 1f );
		SceneObject.Attributes.Set( "maskOffset", new Vector2( maskOffset.x, maskOffset.y ) );

		return true;
	}

	private bool DoTucking()
	{
		var tuckDist = Weapon.GetTuckDist();
		if ( tuckDist != -1 )
		{
			var t = Math.Min( 1, ( ( 32f - tuckDist ) / 32f ) + 0.5f );
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
		ShootRotation = Rotation.Lerp( ShootRotation, Rotation.Identity, 20f * Time.Delta );

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
		swingOffset += swingVelocity * SwingInfluence;

		if ( swingOffset.Length > MaxOffsetLength )
		{
			swingOffset = swingOffset.Normal * MaxOffsetLength;
		}

		return swingOffset;
	}

	private float currentBob = 0;
	protected Vector3 CalcBobbingOffset( Vector3 velocity ) => Bobbing.CalcBobbingOffset( ref currentBob, velocity, Owner );
}
