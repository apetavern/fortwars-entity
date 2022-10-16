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

			// This is an awful way of doing this
			var playerAndChildren = player.Children.ToList().Append( player );

			foreach ( var child in playerAndChildren )
			{
				var glow = child.Components.GetOrCreate<Glow>();

				if ( player.TeamID != localPlayer.TeamID )
				{
					glow.Enabled = true;
					glow.ObscuredColor = new Color( 0, 0, 0, 0 );
					glow.Color = new Color( 1, 0.05f, 0.05f );
					glow.Width = 0.05f;
				}
				else
				{
					glow.Enabled = false;
				}
			}
		} );
	}
}
