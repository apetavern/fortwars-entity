// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class LobbyRound : BaseRound
{
	public override string RoundName => "Lobby";
	public override int RoundDuration => 60;

	protected override void OnStart()
	{
		Log.Info( "Started Lobby Round" );

		if ( Game.IsServer )
		{
			Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( ( player ) =>
			{
				player?.Respawn();
				player?.Inventory.Add( new Grenade() );
			} );
		}
	}

	protected override void OnFinish()
	{
		Log.Info( "Finished Lobby Round" );
	}

	protected override void OnTimeUp()
	{
		if ( Game.Clients.Count >= FortwarsGame.Instance.MinPlayers )
			FortwarsGame.Instance.ChangeRound( new BuildRound() );
		else
			FortwarsGame.Instance.ChangeRound( new LobbyRound() );
	}

	public override void OnPlayerKilled( FortwarsPlayer player )
	{
		player.Respawn();

		base.OnPlayerKilled( player );
	}

	public override void OnPlayerSpawn( FortwarsPlayer player )
	{
		base.OnPlayerSpawn( player );
	}
}
