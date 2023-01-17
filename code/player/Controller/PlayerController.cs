namespace Fortwars;

public partial class PlayerController : EntityComponent<Player>, ISingletonComponent
{
	public Player Player => Entity;

	public Vector3 Position { get; set; }
	public Vector3 Velocity { get; set; }
	public Vector3 BaseVelocity { get; set; }
	public Vector3 WishVelocity { get; set; }
	public Entity GroundEntity { get; set; }
	public Vector3 GroundNormal { get; set; }
	public float CurrentGroundAngle { get; set; }

	public static float BodyGirth => 32f;
	public static float EyeHeight => 64f;

	[Net, Predicted]
	public float CurrentEyeHeight { get; set; } = 64f;

	public IEnumerable<PlayerControllerMechanic> Mechanics => Entity.Components.GetAll<PlayerControllerMechanic>();
	public PlayerControllerMechanic BestMechanic => Mechanics.OrderByDescending( x => x.SortOrder ).FirstOrDefault( x => x.IsActive );

	public BBox Hull
	{
		get
		{
			var girth = BodyGirth * 0.5f;
			var baseHeight = CurrentEyeHeight;

			var mins = new Vector3( -girth, -girth, 0 );
			var maxs = new Vector3( +girth, +girth, baseHeight * 1.1f );

			return new BBox( mins, maxs );
		}
	}

	protected void SimulateEyes()
	{
		Player.EyeRotation = Player.LookInput.ToRotation();
		Player.EyeLocalPosition = Vector3.Up * CurrentEyeHeight;
	}

	public virtual void Simulate( IClient client )
	{
		SimulateEyes();
		SimulateMechanics();

		if ( Debug )
		{
			var hull = Hull;
			DebugOverlay.Box( Position, hull.Mins, hull.Maxs, Color.Red );
			DebugOverlay.Box( Position, hull.Mins, hull.Maxs, Color.Blue );

			var lineOffset = 0;

			DebugOverlay.ScreenText( $"Player Controller", ++lineOffset );
			DebugOverlay.ScreenText( $"        Position: {Position}", ++lineOffset );
			DebugOverlay.ScreenText( $"        Velocity: {Velocity}", ++lineOffset );
			DebugOverlay.ScreenText( $"    BaseVelocity: {BaseVelocity}", ++lineOffset );
			DebugOverlay.ScreenText( $"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]", ++lineOffset );
			DebugOverlay.ScreenText( $"           Speed: {Velocity.Length}", ++lineOffset );

			++lineOffset;
			DebugOverlay.ScreenText( $"Mechanics", ++lineOffset );
			foreach ( var mechanic in Mechanics )
			{
				DebugOverlay.ScreenText( $"{mechanic}", ++lineOffset );
			}
		}
	}

	public virtual void FrameSimulate( IClient client )
	{
		SimulateEyes();
	}

	protected void SimulateMechanics()
	{
		foreach ( var mechanic in Mechanics )
		{
			mechanic.TrySimulate( this );
		}
	}

	public virtual TraceResult TraceBBox( 
		Vector3 start, 
		Vector3 end, 
		Vector3 mins,
		Vector3 maxs, 
		float liftFeet = 0.0f,
		float liftHead = 0.0f)
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		if ( liftHead > 0 )
		{
			end += Vector3.Up * liftHead;
		}

		var tr = Trace.Ray( start, end )
					.Size( mins, maxs )
					.WithAnyTags( "solid", "playerclip", "passbullets", "player" )
					.Ignore( Player )
					.Run();

		return tr;
	}

	public virtual TraceResult TraceBBox( 
		Vector3 start, 
		Vector3 end, 
		float liftFeet = 0.0f,
		float liftHead = 0.0f )
	{
		var hull = Hull;
		return TraceBBox( start, end, hull.Mins, hull.Maxs, liftFeet, liftHead );
	}

	public Vector3 GetWishInput()
	{
		var result = new Vector3( Player.MoveInput.x, Player.MoveInput.y, 0 );

		result *= Player.LookInput.WithPitch( 0f ).ToRotation();

		return result;
	}

	public Vector3 GetWishVelocity( bool zeroPitch = false )
	{
		var result = GetWishInput();
		var inSpeed = result.Length.Clamp( 0, 1 );

		if ( zeroPitch )
			result.z = 0;

		result = result.Normal * inSpeed;
		result *= GetWishSpeed();

		var ang = CurrentGroundAngle.Remap( 0, 45, 1, 0.6f );
		result *= ang;

		return result;
	}

	public virtual float GetWishSpeed()
	{
		return BestMechanic?.WishSpeed ?? 250f;
	}

	public void Accelerate( Vector3 wishdir, float wishspeed, float speedLimit, float acceleration )
	{
		if ( speedLimit > 0 && wishspeed > speedLimit )
			wishspeed = speedLimit;

		var currentspeed = Velocity.Dot( wishdir );
		var addspeed = wishspeed - currentspeed;

		if ( addspeed <= 0 )
			return;

		var accelspeed = acceleration * Time.Delta * wishspeed;

		if ( accelspeed > addspeed )
			accelspeed = addspeed;

		Velocity += wishdir * accelspeed;
	}

	public void ApplyFriction( float stopSpeed, float frictionAmount = 1.0f )
	{
		var speed = Velocity.Length;
		if ( speed.AlmostEqual( 0f ) ) return;

		var control = ( speed < stopSpeed ) ? stopSpeed : speed;
		var drop = control * Time.Delta * frictionAmount;

		// Scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			Velocity *= newspeed;
		}
	}

	public void StepMove( float groundAngle = 46f, float stepSize = 18f )
	{
		MoveHelper mover = new MoveHelper( Position, Velocity );
		mover.Trace = mover.Trace.Size( Hull )
			.Ignore( Player )
			.WithoutTags( "player" );
		mover.MaxStandableAngle = groundAngle;

		mover.TryMoveWithStep( Time.Delta, stepSize );

		Position = mover.Position;
		Velocity = mover.Velocity;
	}

	public void Move( float groundAngle = 46f )
	{
		MoveHelper mover = new MoveHelper( Position, Velocity );
		mover.Trace = mover.Trace.Size( Hull )
			.Ignore( Player )
			.WithoutTags( "player" );
		mover.MaxStandableAngle = groundAngle;

		mover.TryMove( Time.Delta );

		Position = mover.Position;
		Velocity = mover.Velocity;
	}

	[ConVar.Replicated( "playercontroller_debug" )]
	public static bool Debug { get; set; } = false;
}
