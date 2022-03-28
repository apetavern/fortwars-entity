// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

partial class Game
{
	[Net, Change( nameof( OnRoundChange ) )] public BaseRound Round { get; private set; }

	[Net]
	public int MinPlayers { get; set; } = 2;

	[Net] public int BestOf { get; set; } = 3;
	public int RoundsToWin => MathX.CeilToInt( (float)BestOf / 2 );

	[Net] public int BlueWins { get; set; } = 0;
	[Net] public int RedWins { get; set; } = 0;

	[Net] public Team WinningTeam { get; set; } = Team.Invalid;

	public struct MapVote
	{
		public int MapIndex { get; set; }
		public long PlayerId { get; set; }
		public MapVote( int mapIndex, long playerId )
		{
			MapIndex = mapIndex;
			PlayerId = playerId;
		}
	}

	[Net] public IList<MapVote> MapVotes { get; set; }

	public void ChangeRound( BaseRound round )
	{
		Assert.NotNull( round );

		Round?.Finish();
		Round = round;
		Round?.Start();
	}

	[ServerCmd( "vote_map" )]
	public static void VoteMap( int index )
	{
		var playerId = ConsoleSystem.Caller.PlayerId;
		var mapVotes = Instance?.MapVotes;

		foreach ( var vote in mapVotes )
		{
			if ( vote.PlayerId == playerId ) return;
		}

		Instance?.MapVotes.Add( new MapVote( index, playerId ) );
	}

	public static List<string> GetMaps()
	{
		var packageTask = Package.Fetch( Global.GameIdent, true ).ContinueWith( t =>
		{
			Package package = t.Result;
			return package.GetMeta<List<string>>( "MapList" );
		} );

		return packageTask.Result;
	}
}
