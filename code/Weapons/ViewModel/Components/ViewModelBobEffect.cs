namespace Fortwars;

public sealed class ViewModelBobEffect : EntityComponent<ViewModel>, IViewModelEffect
{


	// Not entirely sure what these do..?
	private Vector3 ShootOffset { get; set; }
	private Rotation ShootRotation { get; set; } = Rotation.Identity;

	public ViewModelBobEffect() { }

	private float currentBob;
	private float lastPitch;
	private float lastYaw;

	public void OnApplyEffect( ref ViewModelSetup setup )
	{
		if ( !Entity.Weapon.IsValid() )
			return;

		var controller = Entity.Pawn.Controller;
		var rot = setup.Angles;
		var newPitch = rot.Pitch();
		var newYaw = rot.Yaw();

		var playerVelocity = controller.Velocity;
		var offset = CalculateBobbingOffset( ref currentBob, playerVelocity, Entity.Pawn.Controller );
		if ( Entity.Pawn.GroundEntity == null )
		{
			offset += new Vector3( 0, 0, -2.5f );
			newPitch -= 2.5f;
		}

		offset *= 0.1f;

		var offsetMultiplier = 1.0f;
		var rotationMultiplier = 1.0f;

		if ( Entity.Weapon != null )
		{
			rotationMultiplier = 0.5f;
			offsetMultiplier = 0.5f;

			/*
			if ( Weapon.IsAiming )
			{
				rotationMultiplier = Weapon.WeaponAsset.AimedProceduralViewmodelStrength;
				offsetMultiplier = Weapon.WeaponAsset.AimedProceduralViewmodelStrength;
			}
			*/
		}

		offset += ShootOffset * offsetMultiplier;
		var rotationOffset = ShootRotation * rotationMultiplier;

		var t = playerVelocity.Length.LerpInverse( 0, 350 );
		const float factor = 0.1f;

		offset += new Vector3( t, 0, t / 2f ) * -4f * factor;

		setup.Offset += offset + Entity.Weapon.PositionOffset;
		setup.Angles *= rotationOffset;

		lastPitch = newPitch;
		lastYaw = newYaw;

		var maskOffset = new Vector2( offset.y, offset.z ) * 0.1f * ( 10 * offset.x + 1f );
		Entity.SceneObject.Attributes.Set( "maskOffset", new Vector2( maskOffset.x, maskOffset.y ) );
	}

	public static Vector3 CalculateBobbingOffset( ref float currentBob, Vector3 velocity, PlayerController controller )
	{
		// Bob up and down based on our walk movement
		var forwardVel = velocity.Cross( Vector3.Up );
		var speed = forwardVel.Length.LerpInverse( 0, 400 );
		var speed2 = forwardVel.Length.LerpInverse( 200, 400 );
		var left = Vector3.Left;
		var up = Vector3.Up;

		var applyBob = controller.GroundEntity != null;

		if ( applyBob )
			currentBob += Time.Delta * 25.0f * speed;

		// Reset if we're not really moving
		if ( speed < 0.1f )
			currentBob = currentBob.LerpTo( 0, 5 * Time.Delta );

		// Limit to 1 cycle
		// https://www.desmos.com/calculator/8ued619kst
		currentBob = currentBob.UnsignedMod( MathF.PI * 4f );

		var sprintMul = 2.0f * speed2;
		var speedMul = speed + sprintMul;

		var offset = up * MathF.Sin( currentBob ) * speedMul;
		offset += left * MathF.Sin( currentBob * 0.5f ) * speedMul;

		return offset;
	}
}
