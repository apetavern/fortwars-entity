using Sandbox;

namespace Fortwars
{
	[Library]
	public class DuckSlide : BaseNetworkable
	{

		public PlayerController Controller;

		public bool IsActive; // replicate
		public bool IsActiveSlide;

		public DuckSlide( PlayerController controller )
		{
			Controller = controller;
		}

		float MinimumSlideSpeed => 250;

		public virtual void PreTick()
		{
			bool wants = Input.Down( InputButton.Duck );

			SetEyeHeight();

			if ( wants != IsActive )
			{
				if ( wants )
				{
					if ( Controller.Velocity.Cross( Vector3.Up ).Length > MinimumSlideSpeed && Controller.Velocity.Length > 100 && Controller.GroundEntity != null )
						TrySlide();
					else
						TryDuck();
				}
				else
				{
					TryUnDuck();
				}
			}

			if ( IsActive )
			{
				Controller.SetTag( "ducked" );

				if ( IsActiveSlide )
				{
					Controller.EyePosLocal = Controller.EyePosLocal.LerpTo( new Vector3( 0, 0, 32 ), 7.5f * Time.Delta );
					float t = Controller.Velocity.Dot( Controller.Rotation.Forward ).LerpInverse( 0, 300 );
				}
				else
				{
					Controller.EyePosLocal = Controller.EyePosLocal.LerpTo( new Vector3( 0, 0, 32 ), 25 * Time.Delta );
				}
			}

			if ( IsActiveSlide && Controller.Velocity.Length <= MinimumSlideSpeed )
				TryUnDuck();

			if ( !Controller.GroundEntity.IsValid() )
				IsActiveSlide = false;
		}

		protected virtual void TrySlide()
		{
			IsActive = true;

			float distance = 64;
			var direction = Controller.Pawn.EyeRot.Forward;

			// HACK: Fix - Controller.GroundAngle always returns 46, not sure why.
			float angle = Vector3.GetAngle( Vector3.Up, Controller.GroundNormal );
			float tAngle = angle.LerpInverse( 0, 15f );
			float force = 6.0f.LerpTo( 4.0f, tAngle );
			Controller.Velocity += (direction * distance * force);

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

			if ( IsActiveSlide && Controller.Velocity.Length > MinimumSlideSpeed ) return;

			IsActive = false;
			IsActiveSlide = false;
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
			if ( IsActive && IsActiveSlide ) return 0;
			if ( IsActive ) return 0.5f;
			return 1;
		}

		public virtual float GetFrictionMultiplier()
		{
			// HACK: Fix - Controller.GroundAngle always returns 46, not sure why.
			float angle = Vector3.GetAngle( Vector3.Up, Controller.GroundNormal );

			if ( !IsActiveSlide ) return 1;

			float tAngle = angle.LerpInverse( 15f, 0f );
			float friction = 0.10f.LerpTo( 0.25f, tAngle );

			return friction;
		}

		private void SetEyeHeight()
		{
			if ( IsActive )
			{
				if ( IsActiveSlide )
					eyeHeight = eyeHeight.LerpTo( 0.4f, 20f * Time.Delta );

				eyeHeight = eyeHeight.LerpTo( 0.5f, 10f * Time.Delta );
			}

			eyeHeight = eyeHeight.LerpTo( 1.0f, 50f * Time.Delta );
		}


		float eyeHeight = 1.0f;

		public virtual float GetEyeHeight()
		{
			return eyeHeight;
		}
	}
}
