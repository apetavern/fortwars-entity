namespace Fortwars;

// Come back to this later...
// It is literally just a copy paste from Boomer.
public partial class WeaponViewModel
{
	protected WeaponAsset WeaponAsset => Weapon.WeaponAsset;
	protected ViewModelData Data => WeaponAsset.ViewModelData;

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

	public void AddEffects()
	{
		var player = Weapon.Player;
		var controller = player?.Controller;
		if ( controller == null )
			return;

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

		var mouseDeltaX = -Input.MouseDelta.x * Time.Delta * Data.OverallWeight;
		var mouseDeltaY = -Input.MouseDelta.y * Time.Delta * Data.OverallWeight;

		acceleration += Vector3.Left * mouseDeltaX * -1f;
		acceleration += Vector3.Up * mouseDeltaY * -2f;
		acceleration += -velocity * Data.WeightReturnForce * Time.Delta;

		// Apply horizontal offsets based on walking direction
		var horizontalForwardBob = WalkCycle( 0.5f, 3f ) * speed * Data.WalkCycleOffset.x * Time.Delta;

		acceleration += forward.WithZ( 0 ).Normal.Dot( controller.Velocity.Normal ) * Vector3.Forward * Data.BobAmount.x * horizontalForwardBob;

		// Apply left bobbing and up/down bobbing
		acceleration += Vector3.Left * WalkCycle( 0.5f, 2f ) * speed * Data.WalkCycleOffset.y * ( 1 + sprintLerp ) * ( 1 - aimLerp ) * Time.Delta;
		acceleration += Vector3.Up * WalkCycle( 0.5f, 2f, true ) * speed * Data.WalkCycleOffset.z * ( 1 - aimLerp ) * Time.Delta;
		acceleration += left.WithZ( 0 ).Normal.Dot( controller.Velocity.Normal ) * Vector3.Left * speed * Data.BobAmount.y * Time.Delta * ( 1 - aimLerp );

		velocity += acceleration * Time.Delta;

		ApplyDamping( ref acceleration, Data.AccelerationDamping );
		ApplyDamping( ref velocity, Data.WeightDamping * ( 1 + aimLerp ) );
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
			rotationOffsetTarget *= Rotation.From( Data.GlobalAngleOffset );
			positionOffsetTarget += forward * ( velocity.x * Data.VelocityScale + Data.GlobalPositionOffset.x );
			positionOffsetTarget += left * ( velocity.y * Data.VelocityScale + Data.GlobalPositionOffset.y );
			positionOffsetTarget += up * ( velocity.z * Data.VelocityScale + Data.GlobalPositionOffset.z + upDownOffset );

			float cycle = Time.Now * 10.0f;

			// Crouching
			rotationOffsetTarget *= Rotation.From( Data.CrouchAngleOffset * crouchLerp );
			ApplyPositionOffset( Data.CrouchPositionOffset, crouchLerp );

			// Air
			ApplyPositionOffset( new( 0, 0, 1 ), airLerp );

			// Avoidance
			rotationOffsetTarget *= Rotation.From( Data.AvoidanceAngleOffset * avoidance );
			ApplyPositionOffset( Data.AvoidancePositionOffset, avoidance );

			// Aim Down Sights
			rotationOffsetTarget *= Rotation.From( Data.AimAngleOffset * aimLerp );
			ApplyPositionOffset( Data.AimPositionOffset, aimLerp );

			// Sprinting
			rotationOffsetTarget *= Rotation.From( Data.SprintAngleOffset * sprintLerp );
			ApplyPositionOffset( Data.SprintPositionOffset, sprintLerp );

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

		Rotation *= realRotationOffset;
		Position += realPositionOffset;

		Camera.FieldOfView -= Data.AimFovOffset * aimLerp;
		Camera.Main.SetViewModelCamera( Game.Preferences.FieldOfView, 1, 2048 );
	}
}

public struct ViewModelData
{
	//// General
	public float OverallWeight { get; set; }
	public float WeightReturnForce { get; set; }
	public float WeightDamping { get; set; }
	public float AccelerationDamping { get; set; }
	public float VelocityScale { get; set; }
	public float RotationalPivotForce { get; set; }
	public float RotationalScale { get; set; }

	//// Walking & Bob
	public Vector3 WalkCycleOffset { get; set; }
	public Vector2 BobAmount { get; set; }

	//// Global
	public float GlobalLerpPower { get; set; }
	public Vector3 GlobalPositionOffset { get; set; }
	public Angles GlobalAngleOffset { get; set; }

	//// Crouching
	public Vector3 CrouchPositionOffset { get; set; }
	public Angles CrouchAngleOffset { get; set; }

	//// Avoidance
	/// <summary>
	/// The max position offset when avoidance comes into play.
	/// Avoidance is when something is in the way of the weapon.
	/// </summary>
	public Vector3 AvoidancePositionOffset { get; set; }

	/// <summary>
	/// The max angle offset when avoidance comes into play.
	/// Avoidance is when something is in the way of the weapon.
	/// </summary>
	public Angles AvoidanceAngleOffset { get; set; }

	//// Sprinting
	public Vector3 SprintPositionOffset { get; set; }
	public Angles SprintAngleOffset { get; set; }

	/// Aim Down Sight
	public Vector3 AimPositionOffset { get; set; }
	public Angles AimAngleOffset { get; set; }

	public float AimFovOffset { get; set; }
}
