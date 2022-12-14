// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

partial class FortwarsGame
{
	[Event.Tick.Client]
	private static void CheckForClientVoicePlayed()
	{
		if ( Voice.IsRecording )
			VoiceFeed.Current?.OnVoicePlayed( Game.LocalClient.SteamId, 0 );
	}

	public override void OnVoicePlayed( IClient cl )
	{
		cl.Voice.WantsStereo = false;
		VoiceFeed.Current?.OnVoicePlayed( cl.SteamId, cl.Voice.CurrentLevel );
	}
}
