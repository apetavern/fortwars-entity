namespace Fortwars;

public partial class Weapon
{
	public IEnumerable<WeaponComponent> WeaponComponents => Components.GetAll<WeaponComponent>( includeDisabled: true );

	public bool TryGetComponent<T>( out WeaponComponent comp ) where T : WeaponComponent
	{
		foreach ( var c in WeaponComponents )
		{
			if ( c is T _ )
			{
				comp = (T)c;
				return true;
			}
		}

		comp = null;
		return false;
	}

	public bool HasComponent<T>() where T : WeaponComponent
	{
		foreach ( var c in WeaponComponents )
		{
			if ( c is T _ )
				return true;
		}

		return false;
	}
}
