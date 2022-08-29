// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

partial class Game
{
	[Event.Tick.Client]
	private static void CheckForClientVoicePlayed()
	{
		if ( Voice.IsRecording )
			VoiceFeed.Current?.OnVoicePlayed( Local.PlayerId, 0 );
	}

	public override void OnVoicePlayed( Client cl )
	{
		cl.VoiceStereo = true;
		VoiceFeed.Current?.OnVoicePlayed( cl.PlayerId, cl.VoiceLevel );
	}
}
