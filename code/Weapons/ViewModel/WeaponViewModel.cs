namespace Fortwars;

[Prefab, Category( "Weapons" )]
public partial class WeaponViewModel : AnimatedEntity
{
	public static WeaponViewModel Current { get; set; }

	protected Weapon Weapon { get; set; }

	public WeaponViewModel()
	{
		EnableViewmodelRendering = true;
	}

	public void Attach( Weapon weapon )
	{
		Current = this;

		Weapon = weapon;

		Position = weapon.Player.Position;
		Rotation = weapon.Player.Rotation;
	}

	protected override void OnDestroy()
	{
		Current = null;
	}

	[GameEvent.Client.PostCamera]
	void PlaceViewModel()
	{
		Camera.Main.SetViewModelCamera( 80f );
		AddEffects();
	}

	#region Effects

	// Fields
	Vector3 SmoothedVelocity;
	Vector3 velocity;
	Vector3 acceleration;
	float VelocityClamp => 20f;
	float walkBob = 0;
	float upDownOffset = 0;
	float avoidance = 0;
	float sprintLerp = 0;
	float aimLerp = 0;
	float crouchLerp = 0;
	float airLerp = 0;
	float sideLerp = 0;

	protected float MouseDeltaLerpX;
	protected float MouseDeltaLerpY;

	Vector3 positionOffsetTarget = Vector3.Zero;
	Rotation rotationOffsetTarget = Rotation.Identity;

	Vector3 realPositionOffset;
	Rotation realRotationOffset;

	protected void ApplyPositionOffset( Vector3 offset, float delta )
	{
		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;
		var forward = Camera.Rotation.Forward;

		positionOffsetTarget += forward * offset.x * delta;
		positionOffsetTarget += left * offset.y * delta;
		positionOffsetTarget += up * offset.z * delta;
	}

	private float WalkCycle( float speed, float power, bool abs = false )
	{
		var sin = MathF.Sin( walkBob * speed );
		var sign = Math.Sign( sin );

		if ( abs )
		{
			sign = 1;
		}

		return MathF.Pow( sin, power ) * sign;
	}

	private void LerpTowards( ref float value, float desired, float speed )
	{
		var delta = ( desired - value ) * speed * Time.Delta;
		var deltaAbs = MathF.Min( MathF.Abs( delta ), MathF.Abs( desired - value ) ) * MathF.Sign( delta );

		if ( MathF.Abs( desired - value ) < 0.001f )
		{
			value = desired;

			return;
		}

		value += deltaAbs;
	}

	private void ApplyDamping( ref Vector3 value, float damping )
	{
		var magnitude = value.Length;

		if ( magnitude != 0 )
		{
			var drop = magnitude * damping * Time.Delta;
			value *= Math.Max( magnitude - drop, 0 ) / magnitude;
		}
	}

	protected void AddEffects()
	{
		var player = Weapon.Player;
		var controller = player?.Controller;
		var team = player.Client.Components.Get<TeamComponent>().Team;
		if ( controller == null )
			return;

		BogRoll.SetMaterialGroup( TeamSystem.GetOpposingTeam( team ), this );

		SmoothedVelocity += ( controller.Velocity - SmoothedVelocity ) * 5f * Time.Delta;

		var isGrounded = controller.GroundEntity != null;
		var speed = controller.Velocity.Length.LerpInverse( 0, 750 );
		var bobSpeed = SmoothedVelocity.Length.LerpInverse( -250, 700 );
		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;
		var forward = Camera.Rotation.Forward;
		var isCrouching = false; // controller.IsMechanicActive<CrouchMechanic>();
		var isAiming = false; // Weapon.GetComponent<Aim>()?.IsActive ?? false;

		LerpTowards( ref aimLerp, isAiming ? 1 : 0, isAiming ? 30f : 10f );
		LerpTowards( ref crouchLerp, isCrouching && !isAiming ? 1 : 0, 7f );
		LerpTowards( ref airLerp, ( isGrounded ? 0 : 1 ) * ( 1 - aimLerp ), 10f );

		var leftAmt = left.WithZ( 0 ).Normal.Dot( controller.Velocity.Normal );
		LerpTowards( ref sideLerp, leftAmt * ( 1 - aimLerp ), 5f );

		bobSpeed += sprintLerp * 0.1f;

		if ( isGrounded )
		{
			walkBob += Time.Delta * 30.0f * bobSpeed;
		}
		walkBob %= 360;

		var mouseDeltaX = -Input.MouseDelta.x * Time.Delta * Weight;
		var mouseDeltaY = -Input.MouseDelta.y * Time.Delta * Weight;

		acceleration += Vector3.Left * mouseDeltaX * -1f;
		acceleration += Vector3.Up * mouseDeltaY * -2f;
		acceleration += -velocity * WeightReturnForce * Time.Delta;

		// Apply horizontal offsets based on walking direction
		var horizontalForwardBob = WalkCycle( 0.5f, 3f ) * speed * WalkCycleOffset.x * Time.Delta;

		acceleration += forward.WithZ( 0 ).Normal.Dot( controller.Velocity.Normal ) * Vector3.Forward * BobAmount.x * horizontalForwardBob;

		// Apply left bobbing and up/down bobbing
		acceleration += Vector3.Left * WalkCycle( 0.5f, 2f ) * speed * WalkCycleOffset.y * ( 1 + sprintLerp ) * ( 1 - aimLerp ) * Time.Delta;
		acceleration += Vector3.Up * WalkCycle( 0.5f, 2f, true ) * speed * WalkCycleOffset.z * ( 1 - aimLerp ) * Time.Delta;
		acceleration += left.WithZ( 0 ).Normal.Dot( controller.Velocity.Normal ) * Vector3.Left * speed * BobAmount.y * Time.Delta * ( 1 - aimLerp );
		velocity += acceleration * Time.Delta;

		ApplyDamping( ref acceleration, AccelerationDamping );
		ApplyDamping( ref velocity, WeightDamping * ( 1 + aimLerp ) );
		velocity = velocity.Normal * Math.Clamp( velocity.Length, 0, VelocityClamp );

		var avoidanceTrace = Trace.Ray( Camera.Position, Camera.Position + forward * 50f )
			.WorldAndEntities()
			.WithoutTags( "trigger" )
			.Ignore( Weapon )
			.Ignore( this )
			.Run();

		var avoidanceVal = avoidanceTrace.Hit ? ( 1f - avoidanceTrace.Fraction ) : 0;
		avoidanceVal *= 1 - ( aimLerp * 0.8f );

		LerpTowards( ref avoidance, avoidanceVal, 10f );

		Position = Camera.Position;
		Rotation = Camera.Rotation;

		positionOffsetTarget = Vector3.Zero;
		rotationOffsetTarget = Rotation.Identity;
		{
			// Global
			rotationOffsetTarget *= Rotation.From( GlobalAngleOffset );
			positionOffsetTarget += forward * ( velocity.x * VelocityScale + GlobalPositionOffset.x );
			positionOffsetTarget += left * ( velocity.y * VelocityScale + GlobalPositionOffset.y );
			positionOffsetTarget += up * ( velocity.z * VelocityScale + GlobalPositionOffset.z + upDownOffset );
			float cycle = Time.Now * 10.0f;

			// Crouching
			rotationOffsetTarget *= Rotation.From( CrouchAngleOffset * crouchLerp );
			ApplyPositionOffset( CrouchPositionOffset, crouchLerp );

			// Air
			ApplyPositionOffset( new( 0, 0, 1 ), airLerp );

			// Avoidance
			rotationOffsetTarget *= Rotation.From( AvoidanceAngleOffset * avoidance );
			ApplyPositionOffset( AvoidancePositionOffset, avoidance );

			// Aim Down Sights
			rotationOffsetTarget *= Rotation.From( AimAngleOffset * aimLerp );
			ApplyPositionOffset( AimPositionOffset, aimLerp );

			// Sprinting
			rotationOffsetTarget *= Rotation.From( SprintAngleOffset * sprintLerp );
			ApplyPositionOffset( SprintPositionOffset, sprintLerp );

			// Sprinting Camera Rotation
			Camera.Rotation *= Rotation.From(
				new Angles(
					MathF.Abs( MathF.Sin( cycle ) * 1.0f ),
					MathF.Cos( cycle ),
					0
				) * sprintLerp * 0.3f );
		}

		realRotationOffset = rotationOffsetTarget;
		realPositionOffset = positionOffsetTarget;

		Log.Info( realPositionOffset );

		Rotation *= realRotationOffset;
		Position += realPositionOffset;

		Camera.FieldOfView -= AimFovOffset * aimLerp;
		Camera.Main.SetViewModelCamera( 80f, 1, 2048 );
	}

	#endregion
}
