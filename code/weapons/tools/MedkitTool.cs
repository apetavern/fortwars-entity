// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

[Library( "medkittool", Title = "Medkit" )]
public partial class MedkitTool : DropTool
{
	public override void SpawnPickup()
	{
		ThrowProjectile( new BigHealthPickup() );
	}
}
