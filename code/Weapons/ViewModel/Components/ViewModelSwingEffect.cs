namespace Fortwars;

public sealed class ViewModelSwingEffect : EntityComponent<ViewModel>, IViewModelEffect
{
	private float SwingInfluence { get; init; } = 0.05f;
	private float ReturnSpeed { get; init; } = 5.0f;
	private float MaxOffsetLength { get; init; } = 20.0f;

	public ViewModelSwingEffect() { }

	private Angles lastAngles;

	public void OnApplyEffect( ref ViewModelSetup setup )
	{
		var camera = setup.Camera.Rotation.Angles();
		var angles = ( lastAngles - camera ).Normal * 4;
		var offset = new Vector2( angles.yaw, -angles.pitch );
		lastAngles = camera;

		var swing = CalculateSwingOffset( offset.y, offset.x );
		setup.Offset += setup.Camera.Rotation * swing;
	}

	private Vector3 swingOffset;

	private Vector3 CalculateSwingOffset( float pitchDelta, float yawDelta )
	{
		var swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

		swingOffset -= swingOffset * ReturnSpeed * Time.Delta;
		swingOffset += swingVelocity * SwingInfluence;

		if ( swingOffset.Length > MaxOffsetLength )
		{
			swingOffset = swingOffset.Normal * MaxOffsetLength;
		}

		return swingOffset;
	}
}
