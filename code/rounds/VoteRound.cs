// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.Collections.Generic;

namespace Fortwars;

public partial class VoteRound : BaseRound
{
	public override string RoundName => "Voting";
	public override int RoundDuration => 20;

	protected override void OnStart()
	{
		Log.Info( "Started Vote Round" );

		Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( player =>
		{
			if ( player.LifeState != LifeState.Alive )
				player.RespawnTimer = 0;
		} );
	}

	public override void OnTick()
	{
		base.OnTick();

		// Keep players alive during this round
		Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( player => player.Health = 100 );
	}

	protected override void OnTimeUp()
	{
		var game = FortwarsGame.Instance;
		if ( game == null ) return;

		Dictionary<int, int> voteCount = new();

		foreach ( var vote in game.MapVotes )
		{
			if ( !voteCount.ContainsKey( vote.MapIndex ) )
				voteCount.Add( vote.MapIndex, 0 );

			voteCount[vote.MapIndex]++;
		}

		var sortedMapVotePairs =
			from entry in voteCount
			orderby entry.Value descending
			select entry;

		if ( sortedMapVotePairs.Count() == 0 )
		{
			Game.ChangeLevel( Game.Random.FromList( FortwarsGame.GetMaps() ) );
			return;
		}

		var votedMap = sortedMapVotePairs.First();
		Game.ChangeLevel( FortwarsGame.GetMaps()[votedMap.Key] );
	}
}
