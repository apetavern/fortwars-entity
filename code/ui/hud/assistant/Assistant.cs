// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;

namespace Fortwars;

public partial class Assistant : Panel
{
	public static bool AssistantEnabled { get; set; } = true;

	private Bubble currentBubble;

	private static Assistant Instance { get; set; }
	private TimeSince timeSinceLastSpeech = float.MaxValue;

	public Assistant()
	{
		if ( !AssistantEnabled )
			Delete();

		Instance ??= this;

		StyleSheet.Load( "/ui/hud/assistant/assistant.scss" );

		Speak( "Well! Hello there!", "greeting" );
	}

	[ClientCmd( "assistant_say", CanBeCalledFromServer = true )]
	public static void Speak( string text, string audio = "" )
	{
		if ( !AssistantEnabled )
			return;

		if ( Instance.timeSinceLastSpeech < 0.5f ) // Don't speak too often - annoying
			return;

		Instance.currentBubble?.Delete();
		Instance.currentBubble = new Bubble( text );
		Instance.currentBubble.Parent = Instance;

		Instance.timeSinceLastSpeech = 0;

		if ( !string.IsNullOrEmpty( audio ) )
		{
			_ = Sound.FromScreen( audio ).SetVolume( 4.0f );
		}
	}

	public override void Tick()
	{
		base.Tick();
	}
}
