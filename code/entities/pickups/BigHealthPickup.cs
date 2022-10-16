// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public partial class BigHealthPickup : Deployable
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/items/medkit/medkit_w.vmdl" );
		SetBodyGroup( 0, 1 );
		SetMaterialGroup( "health" );

		Components.Get<BobbingComponent>().NoPitch = true;
	}

	public override void Resupply( FortwarsPlayer player )
	{
		if ( player.Health >= 100 )
			return;

		player.Health += 50f;
		player.Health = player.Health.Clamp( 0, 100 );
	}
}
