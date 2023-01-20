global using Editor;
global using Sandbox;
global using Sandbox.Diagnostics;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using Sandbox.Utility;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

namespace Fortwars;

public partial class FortwarsManager : Sandbox.GameManager
{
	public static FortwarsManager Instance => Current as FortwarsManager;

	public FortwarsManager()
	{
		if ( Game.IsClient )
		{
			_ = new Hud();
		}
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var player = new Player( client );
		client.Pawn = player;
		player.Respawn();

		GamemodeSystem.Instance?.OnClientJoined( client );
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client, reason );

		GamemodeSystem.Instance?.OnClientDisconnect( client, reason );
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		Log.Info( $"Finding spawnpoint for {pawn.Name}" );

		GamemodeSystem.Instance?.MoveToSpawnpoint( pawn );
	}

	[Event.Entity.PostSpawn]
	public void PostEntitySpawn()
	{
		GamemodeSystem.SetupGamemode();
	}
}
