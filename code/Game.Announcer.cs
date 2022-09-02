// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (alex@gu3.me)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

partial class Game
{
	private Queue<string> SoundQueue { get; set; } = new();
	private Sound CurrentlyPlayingSound { get; set; }

	public Player Player { get; set; }

	private void QueueAnnouncerSound( string soundId )
	{
		Host.AssertClient();
		SoundQueue.Enqueue( soundId );
	}

	[Event.Tick.Client]
	public void OnClientTick()
	{
		// Play sounds from the sound queue if we can
		if ( SoundQueue.Count > 0 )
		{
			if ( CurrentlyPlayingSound.Finished )
			{
				var nextSound = SoundQueue.Dequeue();
				CurrentlyPlayingSound = Sound.FromScreen( nextSound );
			}
		}
	}

	[ClientRpc]
	private void RpcInternalPlaySound( string name )
	{
		QueueAnnouncerSound( name );
	}

	public void PlayAnnouncerSound( string name, Team team = Team.Invalid )
	{
		Host.AssertServer();

		var to = ( team == Team.Invalid ) ?
			To.Everyone : ToExtension.Team( team );

		RpcInternalPlaySound( to, name );
	}
}
