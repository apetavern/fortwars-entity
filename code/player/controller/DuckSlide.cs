using Sandbox;

namespace Fortwars
{
	[Library]
	public partial class DuckSlide : BaseNetworkable
	{
		public PlayerController Controller;

		public bool IsActive { get; private set; }
		public bool IsActiveSlide { get; private set; }

		private float MinimumSlideSpeed => 250;
		private TimeSince timeSinceLastSlide = 0;

		public DuckSlide( PlayerController controller )
		{
			Controller = controller;
		}

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
					$"Grounded: {Controller.GroundEntity != null}\n" +
					$"Wants != IsActive: {wants != IsActive}" );
			}

			if ( wants != IsActive )
			{
				if ( wants )
				{
					if ( Controller.Velocity.Length > MinimumSlideSpeed && Controller.GroundEntity != null )
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
				Controller.SetTag( "ducked" );

			if ( IsActiveSlide && Controller.Velocity.Length <= MinimumSlideSpeed )
				TryUnDuck();

			if ( !Controller.GroundEntity.IsValid() )
				TryUnDuck();
		}

		private void TrySlide()
		{
			if ( timeSinceLastSlide < 3 )
				return;

			float distance = 64;
			var direction = Controller.Pawn.EyeRot.Forward;

			Controller.Velocity += (direction * distance * 8.0f);

			IsActive = true;
			IsActiveSlide = true;
		}

		private void TryDuck()
		{
			IsActive = true;
		}

		private void TryUnDuck()
		{
			if ( IsActiveSlide && Controller.Velocity.Length > MinimumSlideSpeed ) return;

			if ( IsActiveSlide )
				timeSinceLastSlide = 0;

			IsActive = false;
			IsActiveSlide = false;
		}

		public float GetWishSpeed()
		{
			if ( IsActive && IsActiveSlide ) return 0;
			if ( IsActive ) return 0.5f;
			return 1;
		}

		public float GetFrictionMultiplier()
		{
			if ( IsActiveSlide ) return 0.2f;
			return 1;
		}

		public float GetEyeHeight()
		{
			if ( IsActive )
			{
				if ( IsActiveSlide )
					return 0.2f;
				else
					return 0.5f;
			}
			else
			{
				return 1.0f;
			}
		}
	}
}
