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

		GamemodeSystem.Instance?.OnClientJoined( client );

		player.Respawn();
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client, reason );

		GamemodeSystem.Instance?.OnClientDisconnect( client, reason );
	}

	[GameEvent.Entity.PostSpawn]
	public void PostEntitySpawn()
	{
		GamemodeSystem.SetupGamemode();
	}

	[ConCmd.Admin( "noclip" )]
	private static void Noclip()
	{
		if ( ConsoleSystem.Caller.Pawn is not Player ply )
			return;

		if ( ply.Controller.TryGetMechanic<NoclipMechanic>( out var noclip ) )
		{
			noclip.Enabled = !noclip.Enabled;
			return;
		}
		else
		{
			var nc = new NoclipMechanic();
			nc.Enabled = true;
			ply.Components.Add( nc );
		}
	}
}
