// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

[Library( "jumppadtool", Title = "Jump Pad" )]
public partial class JumpPadTool : DropTool
{
	public override void SpawnPickup()
	{
		ThrowProjectile( new JumpPad() );
	}
}
