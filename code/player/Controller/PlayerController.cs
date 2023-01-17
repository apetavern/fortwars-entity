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


	public static float BodyGirth => 32f;
	public static float EyeHeight => 64f;

	[Net, Predicted]
	public float CurrentEyeHeight { get; set; } = 64f;


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
	}

	public virtual void FrameSimulate( IClient client )
	{
		SimulateEyes();
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
}
