// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (alex@gu3.me)

using Sandbox;
using System.Linq;

namespace Fortwars;

public static class ToExtension
{
	public static To Team( Team team ) =>
		To.Multiple( Client.All.Where( x => x.Pawn is FortwarsPlayer player && player.TeamID == team ) );
}
