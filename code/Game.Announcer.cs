// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (alex@gu3.me)

using Sandbox;

namespace Fortwars;

partial class Game
{
	[ClientRpc]
	private void RpcInternalPlaySound( string name )
	{
		Log.Trace( $"Playing announcer sound {name}" );
		_ = Sound.FromScreen( name );
	}

	public void PlayAnnouncerSound( string name, Team team = Team.Invalid )
	{
		Host.AssertServer();

		var to = ( team == Team.Invalid ) ?
			To.Everyone : ToExtension.Team( team );

		RpcInternalPlaySound( to, name );
	}
}
