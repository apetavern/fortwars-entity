// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Fortwars;

public class Bubble : Panel
{
	public Bubble( string text )
	{
		StyleSheet.Load( "/ui/hud/assistant/bubble.scss" );

		Add.Label( text );

		_ = DelayedDelete();
	}

	private async Task DelayedDelete()
	{
		await Task.DelaySeconds( 2.5f );
		Delete();
	}
}
