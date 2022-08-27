// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public partial class BigAmmoPickup : Deployable
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/items/medkit/medkit_w.vmdl" );
		SetBodyGroup( 0, 1 );
		SetMaterialGroup( "ammo" );

		Components.Get<BobbingComponent>().NoPitch = true;
	}

	public override void Resupply( FortwarsPlayer player )
	{
		if ( player.ActiveChild == null || !player.ActiveChild.IsValid )
			return;

		if ( player.ActiveChild is not FortwarsWeapon weapon )
			return;

		weapon.ReserveAmmo += weapon.WeaponAsset.MaxAmmo * 2;
	}
}
