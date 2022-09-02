// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public static class ItemUtils
{
	public static Carriable GetItem( string itemName )
	{
		if ( itemName.StartsWith( "fw:" ) )
		{
			return FortwarsWeapon.FromPath( itemName.Remove( 0, 3 ) );
		}
		else
		{
			return TypeLibrary.Create<Carriable>( itemName );
		}
	}

	public static WeaponAsset GetWeaponAsset( string weaponName )
	{
		if ( weaponName.StartsWith( "fw:" ) )
			weaponName = weaponName.Remove( 0, 3 );
		
		return ResourceLibrary.Get<WeaponAsset>( $"{weaponName}" );
	}
}
