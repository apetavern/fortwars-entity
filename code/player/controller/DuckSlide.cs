using Sandbox;

namespace Fortwars
{
	[Library]
	public partial class DuckSlide : BaseNetworkable
	{
		public BasePlayerController Controller;

		public bool IsActive;
		public bool IsActiveSlide;

		public DuckSlide( BasePlayerController controller )
		{
			Controller = controller;
		}

		float MinimumSlideSpeed => 200;

		public virtual void PreTick()
		{
			bool wants = Input.Down( InputButton.Duck );

			if ( wants != IsActive )
			{
				if ( wants )
				{
					if ( Controller.Velocity.Cross( Vector3.Up ).Length > MinimumSlideSpeed && Controller.GroundEntity != null )
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
					Controller.EyePosLocal = Controller.EyePosLocal.LerpTo( new Vector3( 0, 0, 32 ), 50f * Time.Delta );
				else
					Controller.EyePosLocal = Controller.EyePosLocal.LerpTo( new Vector3( 0, 0, 32 ), 25f * Time.Delta );
			}
			else
				Controller.EyePosLocal = Controller.EyePosLocal.LerpTo( new Vector3( 0, 0, 64 ), 25f * Time.Delta );

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

			float force = 8.0f;
			Controller.Velocity += direction * distance * force;

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
			if ( IsActive ) return 200f;
			return -1f;
		}

		public virtual float GetFrictionMultiplier()
		{
			if ( IsActiveSlide ) return 0.25f;
			return 1.0f;
		}
	}
}
