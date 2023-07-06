namespace Fortwars;

[Category( "ViewModel" )]
public class ViewModel : AnimatedEntity
{
	[ConVar.Client( "fw_viewmodel_fov", Help = "ViewModel field of view", Min = 50f, Max = 90f )]
	public static float ViewModelFov { get; set; } = 52f;

	protected float SwingInfluence => 0.05f;
	protected float ReturnSpeed => 5.0f;
	protected float MaxOffsetLength => 20.0f;
	protected Vector3 BobDirection => new( 0.0f, 0.5f, 1.0f );

	private Vector3 TargetPos = 0;
	private Vector3 FinalPos = 0;
	private float TargetFov = ViewModelFov;
	private float FinalFov = ViewModelFov;
	private Rotation TargetRot = Rotation.Identity;
	private Rotation FinalRot = Rotation.Identity;

	private float LerpSpeed = 10f;

	private Vector3 swingOffset;
	private float lastPitch;
	private float lastYaw;
	private float bobAnim;
	private float currentBob;

	private bool activated;

	private Vector3 ShootOffset { get; set; }
	private Rotation ShootRotation { get; set; }

	private Weapon Weapon { get; set; }

	public ViewModel() { }

	public ViewModel( Weapon weapon )
	{
		Weapon = weapon;
	}

	[GameEvent.Client.PostCamera]
	private void PostCamera()
	{
		if ( !Game.LocalPawn.IsValid )
			return;

		var player = Game.LocalPawn as Player;
		var controller = player?.Controller;
		if ( controller is null )
			return;

		if ( !activated )
		{
			lastPitch = Camera.Rotation.Pitch();
			lastYaw = Camera.Rotation.Yaw();

			activated = true;
		}

		Rotation = Camera.Rotation;
		Position = Camera.Position + ( Rotation.Forward * 32f ) + ( Rotation.Up * 2f );

		FinalRot = Rotation.Lerp( FinalRot, TargetRot, LerpSpeed * Time.Delta );
		Rotation *= FinalRot;

		FinalPos = FinalPos.LerpTo( TargetPos, LerpSpeed * Time.Delta );
		Position += FinalPos * Rotation;

		FinalFov = FinalFov.LerpTo( TargetFov, LerpSpeed * Time.Delta );
		Camera.Main.SetViewModelCamera( FinalFov );

		TargetPos = 0;
		TargetFov = ViewModelFov;
		TargetRot = Rotation.Identity;
		LerpSpeed = 20f;

		if ( !Weapon.IsValid )
			return;

		DoBobbing( controller );
	}

	private void DoBobbing( PlayerController controller )
	{
		var newPitch = Rotation.Pitch();
		var newYaw = Rotation.Yaw();

		var pitchDelta = Angles.NormalizeAngle( newPitch - lastPitch );
		var yawDelta = Angles.NormalizeAngle( lastYaw - newYaw );

		var playerVelocity = controller.Velocity;
		var verticalDelta = playerVelocity.z * Time.Delta;
		var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
		verticalDelta *= 1.0f - MathF.Abs( viewDown.Cross( Vector3.Down ).y );
		pitchDelta -= verticalDelta * 1;

		var offset = CalcSwingOffset( pitchDelta, yawDelta );
		offset += CalcBobbingOffset( ref currentBob, playerVelocity, controller );
		if ( Owner.GroundEntity == null )
		{
			offset += new Vector3( 0, 0, -2.5f );
			newPitch -= 2.5f;
		}

		offset *= 0.1f;

		float offsetMultiplier = 1.0f;
		float rotationMultiplier = 1.0f;

		if ( Weapon != null )
		{
			rotationMultiplier = 0.5f;
			offsetMultiplier = 0.5f;

			/*			if ( Weapon.IsAiming )
						{
							rotationMultiplier = Weapon.WeaponAsset.AimedProceduralViewmodelStrength;
							offsetMultiplier = Weapon.WeaponAsset.AimedProceduralViewmodelStrength;
						}*/
		}

		offset += ShootOffset * offsetMultiplier;
		var rotationOffset = ShootRotation * rotationMultiplier;

		float t = playerVelocity.Length.LerpInverse( 0, 350 );
		float factor = 0.1f;

		offset += new Vector3( t, 0, t / 2f ) * -4f * factor;

		TargetPos += offset + Weapon.PositionOffset;
		TargetRot *= rotationOffset;

		lastPitch = newPitch;
		lastYaw = newYaw;

		Vector2 maskOffset = new Vector2( offset.y, offset.z ) * 0.1f * ( 10 * offset.x + 1f );
		SceneObject.Attributes.Set( "maskOffset", new Vector2( maskOffset.x, maskOffset.y ) );
	}

	protected Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
	{
		Vector3 swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

		swingOffset -= swingOffset * ReturnSpeed * Time.Delta;
		swingOffset += swingVelocity * SwingInfluence;

		if ( swingOffset.Length > MaxOffsetLength )
		{
			swingOffset = swingOffset.Normal * MaxOffsetLength;
		}

		return swingOffset;
	}

	public static Vector3 CalcBobbingOffset( ref float currentBob, Vector3 velocity, PlayerController controller )
	{
		// Bob up and down based on our walk movement
		var forwardVel = velocity.Cross( Vector3.Up );
		var speed = forwardVel.Length.LerpInverse( 0, 400 );
		var speed2 = forwardVel.Length.LerpInverse( 200, 400 );
		var left = Vector3.Left;
		var up = Vector3.Up;

		bool applyBob = controller.GroundEntity != null;

		if ( applyBob )
			currentBob += Time.Delta * 25.0f * speed;

		// Reset if we're not really moving
		if ( speed < 0.1f )
		{
			currentBob = currentBob.LerpTo( 0, 5 * Time.Delta );
		}

		// Limit to 1 cycle
		// https://www.desmos.com/calculator/8ued619kst
		currentBob = currentBob.UnsignedMod( MathF.PI * 4f );

		float sprintMul = 2.0f * speed2;
		float speedMul = speed + sprintMul;

		var offset = up * MathF.Sin( currentBob ) * speedMul;
		offset += left * MathF.Sin( currentBob * 0.5f ) * speedMul;

		return offset;
	}
}
