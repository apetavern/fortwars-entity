// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class VoiceFeed : Panel
{
	public static VoiceFeed Current { get; internal set; }

	public VoiceFeed()
	{
		Current = this;

		StyleSheet.Load( "/ui/hud/VoiceFeed.scss" );
	}

	public void OnVoicePlayed( long steamId, float level )
	{
		var entry = ChildrenOfType<VoiceEntry>().FirstOrDefault( x => x.Friend.Id == steamId );
		if ( entry == null ) entry = new VoiceEntry( this, steamId );

		entry.Update( level );
	}
}
