// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.Component;
using System.Linq;

namespace Fortwars;

public partial class FortwarsPlayer
{
	[Event.Tick.Client]
	public static void OnClientTick()
	{
		var localPlayer = Local.Pawn as FortwarsPlayer;

		All.OfType<FortwarsPlayer>().ToList().ForEach( player =>
		{
			if ( player.IsLocalPawn )
				return;

			var glow = player.Components.GetOrCreate<Glow>();

			if ( player.TeamID != localPlayer.TeamID )
			{
				glow.Active = true;
				glow.RangeMin = 0;
				glow.RangeMax = int.MaxValue;
				glow.Color = Color.Red;
			}
			else
			{
				glow.Active = false;
			}
		} );
	}
}
