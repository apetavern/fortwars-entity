// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;

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

	[ClientCmd( "fw_message_add", CanBeCalledFromServer = true )]
	public static void AddMessage( string icon, string title, string message, bool priority = false )
	{
		if ( !priority && Instance.timeSinceLastMessage < 3 )
			return;

		Instance.Add.Message( icon, title, message );
		Instance.timeSinceLastMessage = 0;
	}
}
