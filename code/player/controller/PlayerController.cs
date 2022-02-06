using Sandbox;

namespace Fortwars
{
	public class PlayerController : BasePlayerController
	{
		public override void FrameSimulate()
		{
			base.FrameSimulate();

			SetEyeTransform();
		}

		TimeSince CoyoteTime = 0;

		//
		// Movement properties
		//
		private float Gravity => 800f;
		private float MoveSpeed => 400f;
		private float Accel => 7.5f;
		private float SprintAccel => 10f;
		private float AirAccel => 1.0f;
		private float JumpHeight => 300f;
		private float Friction => 8f;
		private float Drag => 2.0f;
		private float Height => 64f;

		//
		// Runtime
		//
		private bool IsGrounded => GroundEntity != null;
		private bool IsSwimming => Pawn.WaterLevel.Fraction > 0.1f;
		public float ForwardSpeed => Velocity.Dot( EyeRot.Forward );
		public bool IsSprinting => Input.Down( InputButton.Run ) && IsGrounded && ForwardSpeed > 100f;


		public DuckSlide DuckSlide { get; private set; }

		public PlayerController()
		{
			DuckSlide = new( this );
		}

		private MoveHelper CreateMoveHelper()
		{
			var moveHelper = new MoveHelper( Position, Velocity );

			moveHelper.Trace = moveHelper.Trace
									  .Size( new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 64 ) )
									  .Ignore( Pawn );

			moveHelper.GroundBounce = 0.0f;
			moveHelper.WallBounce = 0.2f;
			moveHelper.MaxStandableAngle = 60f;

			return moveHelper;
		}

		public override void Simulate()
		{
			base.Simulate();

			SetEyeTransform();
			DuckSlide.PreTick();

			var moveHelper = CreateMoveHelper();

			CheckGrounded( ref moveHelper );

			if ( !IsGrounded && !IsSwimming )
				moveHelper.Velocity += Vector3.Down * Gravity * Time.Delta;

			if ( Input.Pressed( InputButton.Jump ) && (IsGrounded || CoyoteTime < 0.2f) )
				Jump( ref moveHelper );

			moveHelper.Velocity += GetWishDirection() * MoveSpeed * GetAccel() * DuckSlide.GetWishSpeed() * Time.Delta;

			// Save Z velocity because we don't want to apply friction to it
			float startZ = moveHelper.Velocity.z;
			moveHelper.ApplyFriction( GetFriction(), Time.Delta );
			moveHelper.Velocity = moveHelper.Velocity.WithZ( startZ );

			if ( IsGrounded )
				moveHelper.Velocity = moveHelper.Velocity.WithZ( 0 ); // Don't push us into the ground if we're on it

			moveHelper.TryMoveWithStep( Time.Delta, 16f );
			moveHelper.TryUnstuck();

			Position = moveHelper.Position;
			Velocity = moveHelper.Velocity;

			if ( IsGrounded && !IsSwimming )
				StayOnGround( ref moveHelper );

			if ( Debug )
			{
				DebugOverlay.ScreenText( 0, $"Position: {Position}" );
				DebugOverlay.ScreenText( 1, $"Velocity: {Velocity}" );
				DebugOverlay.ScreenText( 2, $"Grounded: {IsGrounded}" );
				DebugOverlay.ScreenText( 3, $"Forward speed: {ForwardSpeed}" );
			}
		}

		private float GetAccel()
		{
			float weaponFactor = 1.0f;
			if ( Pawn.ActiveChild is FortwarsWeapon weapon )
				weaponFactor = weapon.WeaponAsset.MovementSpeedMultiplier;

			if ( !IsGrounded )
				return AirAccel * weaponFactor;

			if ( Input.Down( InputButton.Run ) )
				return SprintAccel * weaponFactor;

			return Accel * weaponFactor;
		}
		private float GetFriction() => (IsGrounded ? Friction : Drag) * DuckSlide.GetFrictionMultiplier();

		private void SetEyeTransform()
		{
			EyePosLocal = Vector3.Up * Height * DuckSlide.GetEyeHeight();
			EyeRot = Input.Rotation;
		}

		private Vector3 GetWishDirection()
		{
			Vector3 dir = 0;
			var eyeRotYawOnly = EyeRot.Angles().WithRoll( 0 ).WithPitch( 0 ).ToRotation();
			dir += Input.Forward * eyeRotYawOnly.Forward;
			dir += Input.Left * eyeRotYawOnly.Left;

			if ( dir.Length > 1f )
				dir = dir.Normal;

			return dir;
		}

		private void Jump( ref MoveHelper moveHelper )
		{
			GroundEntity = null;

			float groundFactor = 1.0f;
			if ( IsSwimming )
				groundFactor = 0.0f;

			moveHelper.Velocity = Velocity.WithZ( JumpHeight * groundFactor );
			moveHelper.Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

			AddEvent( "jump" );
		}

		public virtual void StayOnGround( ref MoveHelper moveHelper )
		{
			var start = Position + Vector3.Up * 2;
			var end = Position + Vector3.Down * 16f;

			var trace = moveHelper.TraceFromTo( Position, start );
			start = trace.EndPos;

			// Now trace down from a known safe position
			trace = moveHelper.TraceFromTo( start, end );

			if ( trace.Fraction <= 0 ) return;
			if ( trace.Fraction >= 1 ) return;
			if ( trace.StartedSolid ) return;
			if ( Vector3.GetAngle( Vector3.Up, trace.Normal ) > 45f ) return;

			Position = trace.EndPos;
		}

		private void CheckGrounded( ref MoveHelper moveHelper )
		{
			bool wasGrounded = IsGrounded;

			var groundTrace = moveHelper.TraceDirection( Vector3.Down * 2 );
			GroundEntity = null;

			if ( groundTrace.Hit )
				GroundEntity = groundTrace.Entity;

			if ( wasGrounded && !IsGrounded )
				CoyoteTime = 0;
		}
	}
}
