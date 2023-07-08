namespace Fortwars;

public struct ViewModelSetup
{
	public ViewModelSetup( Transform camera )
	{
		Camera = camera;
		
		Offset = Vector3.Zero;
		Angles = Rotation.Identity;
		FovOffset = 0;
	}
	
	public Transform Camera { get; }
	
	public Vector3 Offset;
	public Rotation Angles;
	public float FovOffset;
}
