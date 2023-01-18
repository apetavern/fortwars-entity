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

	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var player = new Player( client );
		client.Pawn = player;
		player.Respawn();
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		Log.Info( $"Finding spawnpoint for {pawn.Name}" );

		var spawnpoints = All.OfType<InfoPlayerTeamspawn>();
		var randomSpawn = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		if ( randomSpawn == null )
		{
			Log.Warning( "Couldn't find spawnpoint!" );
			return;
		}

		pawn.Position = randomSpawn.Position;
		pawn.Rotation = randomSpawn.Rotation;
	}
}
