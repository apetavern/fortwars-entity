// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public class AmmoPickup : Pickup
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/rust_props/small_junk/carton_box.vmdl" );
		Scale = 1.5f;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !IsServer )
			return;

		if ( !( other as FortwarsPlayer ).IsValid() ) return;

		if ( ( other as FortwarsPlayer ).ActiveChild is not FortwarsWeapon weapon )
			return;

		weapon.ReserveAmmo += weapon.WeaponAsset.MaxAmmo * 2;

		Delete();
	}
}
