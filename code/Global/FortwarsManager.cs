namespace Fortwars;

public class FortwarsManager : GameManager
{
	public static FortwarsManager Instance => Current as FortwarsManager;

	public FortwarsManager()
	{
		if ( Game.IsClient )
		{
			_ = new UI.Hud();
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
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		if ( player.Controller.TryGetMechanic<NoclipMechanic>( out var noclip ) )
		{
			noclip.Enabled = !noclip.Enabled;
			return;
		}
		
		var nc = new NoclipMechanic { Enabled = true };
		player.Components.Add( nc );
	}
}
