namespace Fortwars;

/// <summary>
/// A ViewModel is the representation of a weapon in first person. It is a client-side entity that is attached to the player's camera.
/// This gives off the illusion that the player is holding a weapon.
/// </summary>
[Title( "View Model" ), Category( "ViewModel" )]
public sealed class ViewModel : AnimatedEntity
{
	public Weapon Weapon { get; }

	public ViewModel( Weapon weapon )
	{
		Game.AssertClient();
		Weapon = weapon;
	}

	public override void Spawn()
	{
		base.Spawn();
		
		// This shouldn't be defined here, but it is here for now.
		Components.Create<ViewModelBobEffect>();
		Components.Create<ViewModelSwingEffect>();
	}

	public List<IViewModelEffect> Effects { get; } = new();

	protected override void OnComponentAdded( EntityComponent component )
	{
		if ( component is IViewModelEffect effect )
			Effects.Add( effect );
	}

	protected override void OnComponentRemoved( EntityComponent component )
	{
		if ( component is IViewModelEffect effect )
			Effects.Remove( effect );
	}

	public Player Pawn { get; private set; }

	[GameEvent.Client.PostCamera]
	private void OnPostCamera()
	{
		if ( Weapon.Root is not Player player || !player.IsValid() )
			return;

		// Allow view model effects to get the pawn easily 
		Pawn = player;

		Rotation = Camera.Rotation;
		Position = Camera.Position;

		var setup = new ViewModelSetup( new Transform( Camera.Position, Camera.Rotation ) );
		foreach ( var effect in Effects )
		{
			effect.OnApplyEffect( ref setup );
		}

		Position += setup.Offset;
		Rotation *= setup.Angles;

		Camera.Main.SetViewModelCamera( Commands.ViewModelFov + setup.FovOffset );
	}
}
