using Sandbox;

namespace Fortwars
{
	[Library]
	public partial class DuckSlide : BaseNetworkable
	{
		public PlayerController Controller;

		public bool IsActive { get; set; }
		public bool IsActiveSlide { get; set; }

		public DuckSlide( PlayerController controller )
		{
			Controller = controller;
		}

		float MinimumSlideSpeed => 250;

		public virtual void PreTick()
		{
			bool wants = Input.Down( InputButton.Duck );

			if ( BasePlayerController.Debug )
			{
				DebugOverlay.ScreenText( new Vector2( 32, (Host.IsClient) ? 250 : 500 ),
					$"Server: {Host.IsServer}\n" +
					$"IsActive: {IsActive}\n" +
					$"IsActiveSlide: {IsActiveSlide}\n" +
					$"Wants: {wants}\n" +
					$"Controller Velocity: {Controller.Velocity.Length}\n" +
					$"Grounded: {Controller.GroundEntity != null}" );
			}

			if ( wants != IsActive )
			{
				if ( wants )
				{
					if ( Controller.Velocity.Length > 100 && Controller.GroundEntity != null )
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
			Controller.Velocity += (direction * distance * 8.0f);

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
			float friction = 0.2f;

			return friction;
		}

		public virtual float GetEyeHeight()
		{
			if ( IsActive )
			{
				if ( IsActiveSlide )
				{
					return 0.2f;
				}
				else
				{
					return 0.5f;
				}
			}
			else
			{
				return 1.0f;
			}
		}
	}
}
