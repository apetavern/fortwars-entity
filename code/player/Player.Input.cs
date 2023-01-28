using System.ComponentModel;

namespace Fortwars;

public partial class Player
{
	[ClientInput]
	public Vector2 MoveInput { get; protected set; }

	[ClientInput]
	public Angles LookInput { get; protected set; }

	[ClientInput]
	public Entity ActiveWeaponInput { get; set; }

	[Browsable( false )]
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	[Net, Predicted, Browsable( false )]
	public Vector3 EyeLocalPosition { get; set; }

	[Browsable( false )]
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Rotation EyeLocalRotation { get; set; }

	/// <summary>
	/// Override the aim ray to use the player's eye position and rotation.
	/// </summary>
	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );

	public override void BuildInput()
	{
		if ( Input.StopProcessing )
			return;

		Inventory?.BuildInput();
		if ( Input.StopProcessing )
			return;

		MoveInput = Input.AnalogMove;

		var lookInput = ( LookInput + Input.AnalogLook ).Normal;
		LookInput = lookInput.WithPitch( lookInput.pitch.Clamp( -90f, 90f ) );
	}
}
