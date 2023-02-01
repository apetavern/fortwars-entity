namespace Fortwars;

public partial class Weapon
{
	public T GetComponent<T>() where T : WeaponComponent
	{
		return Components.Get<T>( false );
	}

	public override void BuildInput()
	{
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.BuildInput();
		}
	}

	protected void SimulateComponents( IClient cl )
	{
		var player = Owner as Player;
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.Simulate( cl, player );
		}
	}

	protected void CreateComponents()
	{
		if ( WeaponAsset.Components == null )
			return;

		foreach ( var name in WeaponAsset.Components )
		{
			var component = TypeLibrary.Create<WeaponComponent>( name );
			if ( component == null )
				continue;

			component.Initialize( this );
			Components.Add( component );
		}
	}

	public void RunGameEvent( string eventName )
	{
		Player?.RunGameEvent( eventName );
	}
}
