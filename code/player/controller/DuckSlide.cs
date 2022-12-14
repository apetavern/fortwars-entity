// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

[Library]
public partial class DuckSlide : BaseNetworkable
{
	public FortwarsWalkController Controller;

	public bool IsActive { get; private set; }
	public bool IsActiveSlide { get; private set; }

	private float MinimumSlideSpeed => 150f;
	private float SlideExitSpeed => 320f;
	private float SlideForce => 400f;

	TimeSince timeSinceSlide = 0;
	TimeSince timeSinceDuck = 0;

	public DuckSlide( FortwarsWalkController controller )
	{
		Controller = controller;
	}

	public virtual void PreTick()
	{
		bool wants = Input.Down( InputButton.Duck );

		if ( wants != IsActive )
		{
			if ( wants )
			{
				if ( Controller.ForwardSpeed > MinimumSlideSpeed && Controller.GroundEntity != null )
					TrySlide();
				else
					TryDuck();
			}
			else
			{
				TryUnDuck();
			}
		}

		UpdateEyePosition();

		if ( IsActive )
			Controller.SetTag( "ducked" );

		if ( IsActiveSlide )
		{
			// TODO
			// _ = new Sandbox.ScreenShake.Perlin( 1, 1, 0.25f, 0.6f );

			if ( Controller.Velocity.Length <= SlideExitSpeed )
				TryUnDuck();

			Controller.SetTag( "slide" );
		}

		if ( !Controller.GroundEntity.IsValid() )
			IsActiveSlide = false;
	}

	private void UpdateEyePosition()
	{
		if ( IsActive )
		{
			float lerpRate = 25f;

			if ( IsActiveSlide )
				lerpRate = 5f;

			Controller.EyeLocalPosition = Controller.EyeLocalPosition.LerpTo( new Vector3( 0, 0, 32 ), lerpRate * Time.Delta );
		}
		else
		{
			float lerpRate = 25f;

			if ( timeSinceSlide < 1f )
				lerpRate = 5f;

			Controller.EyeLocalPosition = Controller.EyeLocalPosition.LerpTo( new Vector3( 0, 0, 64 ), lerpRate * Time.Delta );
		}
	}

	protected virtual void TrySlide()
	{
		if ( timeSinceSlide < 2 )
			return;

		IsActive = true;

		float mul = ( Controller.Pawn as FortwarsPlayer )?.Class.SpeedMultiplier ?? 1.0f;

		var direction = ( Controller.Pawn as FortwarsPlayer ).EyeRotation.Forward;
		Controller.Velocity += direction * SlideForce * mul;

		IsActiveSlide = true;
	}

	protected virtual void TryDuck()
	{
		IsActive = true;
	}

	protected virtual void TryUnDuck()
	{
		var pm = Controller.TraceBBox( Controller.Position, Controller.Position, originalMins, originalMaxs );
		if ( pm.StartedSolid ) return;

		if ( IsActiveSlide && Controller.Velocity.Length > SlideExitSpeed ) return;

		if ( IsActiveSlide )
			timeSinceSlide = 0;
		else
			timeSinceDuck = 0;

		IsActive = false;
		IsActiveSlide = false;
	}

	public void CancelSlide()
	{
		TryUnDuck();
	}

	// Uck, saving off the bbox kind of sucks
	// and we should probably be changing the bbox size in PreTick
	Vector3 originalMins;
	Vector3 originalMaxs;

	public virtual void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
	{
		originalMins = mins;
		originalMaxs = maxs;

		if ( IsActive )
			maxs = maxs.WithZ( 36 * scale );
	}

	public virtual float GetWishSpeed()
	{
		if ( IsActiveSlide ) return 0f;
		if ( IsActive ) return 200f;
		return -1f;
	}

	public virtual float GetFrictionMultiplier()
	{
		if ( IsActiveSlide ) return 0.125f;
		return 1.0f;
	}
}
