// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public static class ToExtension
{
	public static To Team( Team team ) =>
		To.Multiple( Game.Clients.Where( x => x.Pawn is FortwarsPlayer player && player.TeamID == team ) );
}
