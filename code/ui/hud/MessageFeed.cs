// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public partial class MessageFeed : Panel
{
	public static MessageFeed Instance { get; set; }
	private TimeSince timeSinceLastMessage = 0;

	public MessageFeed()
	{
		Instance = this;
		StyleSheet.Load( "/ui/hud/MessageFeed.scss" );
	}

	[ConCmd.Client( "fw_message_add", CanBeCalledFromServer = true )]
	public static void AddMessage( string icon, string message, bool priority = false )
	{
		if ( !priority && Instance.timeSinceLastMessage < 3 )
			return;

		Instance.Add.Message( icon, message );
		Instance.timeSinceLastMessage = 0;
	}
}
